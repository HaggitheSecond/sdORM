using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Session;

namespace sdORM.MySql
{
    public class ExpressionToMySqlProvider : IExpressionToSqlProvider
    {
        public ParameterizedSql BuildSqlQuery<T>(Expression<Func<T, bool>> expression)
        {
            return new SpookyVisitor().ParseExpressionTree(expression);
        }

        private class SpookyVisitor : ExpressionVisitor
        {
            private readonly IList<string> _expressionParts;
            private readonly IList<KeyValuePair<string, object>> _parameters;

            // This is not a very nice way to do this... TODO: Rework this (again)
            private int _ignoreNextMemberCount;
            private int _ignoreNextConstantCount;

            public SpookyVisitor()
            {
                this._expressionParts = new List<string>();
                this._parameters = new List<KeyValuePair<string, object>>();

            }

            public ParameterizedSql ParseExpressionTree(Expression expression)
            {
                this.Visit(expression);

                return new ParameterizedSql
                {
                    Sql = this.ConvertPrefixToInfix(this._expressionParts),
                    Parameters = this._parameters.Select(f => new SqlParameter
                    {
                        ParameterName = f.Key,
                        Value = f.Value
                    }).ToList()
                };
            }

            private string ConvertPrefixToInfix(IList<string> parts)
            {
                var stack = new Stack<string>();

                foreach (var currentPart in parts.Reverse().ToList())
                {
                    stack.Push(this.IsOperand(currentPart) ? $"({stack.Pop()} {currentPart} {stack.Pop()})" : currentPart);
                }

                var infixResult = stack.Pop();

                infixResult = infixResult.Remove(0, 1);
                infixResult = infixResult.Remove(infixResult.Length - 1, 1);

                return infixResult;
            }

            private bool IsOperand(string value)
            {
                switch (value)
                {
                    case "=":
                    case ">":
                    case "<":
                    case ">=":
                    case "<=":
                    case "<>":

                    case "OR":
                    case "AND":

                    case "IS":
                    case "LIKE":
                    case "IN":
                        return true;
                    default:
                        return false;
                }
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                this._expressionParts.Add(node.NodeType.ToSqlOperator());
                return base.VisitBinary(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (this._ignoreNextMemberCount != 0)
                {
                    this._ignoreNextMemberCount--;
                }
                else
                {
                    if (this.IsOperand(this._expressionParts.Last()))
                    {
                        this._expressionParts.Add(node.Member.Name);
                    }
                    else
                    {
                        if (node.Member.DeclaringType == typeof(DateTime))
                        {
                            if (node.Member.Name == "Today")
                            {
                                var latest = $"DATE({this._expressionParts.Last()})";
                                this._expressionParts.RemoveAt(this._expressionParts.Count - 1);
                                this._expressionParts.Add(latest);
                                this._expressionParts.Add("DATE(NOW())");
                            }

                            if (node.Member.Name == "Now")
                            {
                                this._expressionParts.Add("NOW()");
                            }
                        }
                        else
                        {
                            var value = Expression.Lambda(node).Compile().DynamicInvoke();

                            var parameterName = this.GetUnusedParameterName(this._expressionParts.Last());
                            this._parameters.Add(new KeyValuePair<string, object>(parameterName, value));
                            this._expressionParts.Add(parameterName);

                            this._ignoreNextConstantCount = 1;
                        }
                    }
                }

                return base.VisitMember(node);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (this._ignoreNextConstantCount != 0)
                {
                    this._ignoreNextConstantCount--;
                }
                else
                {
                    var parameterName = this.GetUnusedParameterName(this._expressionParts.Last());
                    this._parameters.Add(new KeyValuePair<string, object>(parameterName, node.Value.ToString()));
                    this._expressionParts.Add(parameterName);
                }

                return base.VisitConstant(node);
            }

            protected override Expression VisitNew(NewExpression node)
            {
                var value = Expression.Lambda(node).Compile().DynamicInvoke();

                if (value == null || (value as IList)?.Count == 0)
                {

                }
                else
                {
                    var parameterName = this.GetUnusedParameterName(this._expressionParts.Last());
                    this._parameters.Add(new KeyValuePair<string, object>(parameterName, value));
                    this._expressionParts.Add(parameterName);

                    this._ignoreNextConstantCount = node.Arguments.Count;
                }

                return base.VisitNew(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.Name == "IsNullOrWhiteSpace")
                {
                    var temp = (MemberExpression)node.Arguments.First();

                    this._expressionParts.Add("OR");
                    this._expressionParts.Add("IS");
                    this._expressionParts.Add(temp.Member.Name);
                    this._expressionParts.Add("NULL");
                    this._expressionParts.Add("=");
                    this._expressionParts.Add(temp.Member.Name);
                    this._expressionParts.Add("' '");

                    this._ignoreNextMemberCount = 1;
                }
                else if (node.Method.Name == "Contains" && node.Method.DeclaringType == typeof(string))
                {
                    var memberExpression = (MemberExpression)node.Object;
                    var valueExpression = (ConstantExpression)node.Arguments.First();

                    this._expressionParts.Add("LIKE");
                    this._expressionParts.Add(memberExpression.Member.Name);

                    var parameterName = this.GetUnusedParameterName(memberExpression.Member.Name);
                    this._parameters.Add(new KeyValuePair<string, object>(parameterName, valueExpression.Value));

                    this._expressionParts.Add($"\"%{parameterName}%\"");

                }
                else if (node.Method.Name == "Contains" && typeof(IEnumerable).IsAssignableFrom(node.Method.DeclaringType))
                {
                    var memberExpression = (MemberExpression)node.Arguments.First();

                    IList values;
                    if (node.Object.NodeType == ExpressionType.ListInit)
                    {
                        values = (IList)Expression.Lambda((ListInitExpression)node.Object).Compile().DynamicInvoke();
                        this._ignoreNextConstantCount = values.Count;
                    }
                    else
                    {
                        values = (IList)Expression.Lambda((MemberExpression)node.Object).Compile().DynamicInvoke();
                        this._ignoreNextConstantCount = 1;
                    }

                    this._expressionParts.Add("IN");
                    this._expressionParts.Add(memberExpression.Member.Name);

                    var parameters = new List<string>();
                    foreach (var currentValue in values)
                    {
                        var parameterName = this.GetUnusedParameterName(memberExpression.Member.Name);
                        this._parameters.Add(new KeyValuePair<string, object>(parameterName, currentValue));

                        parameters.Add(parameterName);
                    }

                    this._expressionParts.Add($"({string.Join(", ", parameters)})");

                    this._ignoreNextMemberCount = 2;
                }
                else
                {
                    // wat to here?
                }

                return base.VisitMethodCall(node);
            }

            protected override Expression VisitListInit(ListInitExpression node)
            {
                return base.VisitListInit(node);
            }

            private string GetUnusedParameterName(string preferedName)
            {
                preferedName = "@" + preferedName;

                if (this._parameters.Select(f => f.Key).Contains(preferedName) == false)
                    return preferedName;

                string newName;
                var i = 1;

                while (true)
                {
                    newName = preferedName + i;

                    if (this._parameters.Select(f => f.Key).Contains(newName) == false)
                        break;

                    i++;
                }

                return newName;
            }


            protected override Expression VisitBlock(BlockExpression node)
            {
                return base.VisitBlock(node);
            }

            protected override CatchBlock VisitCatchBlock(CatchBlock node)
            {
                return base.VisitCatchBlock(node);
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                return base.VisitConditional(node);
            }

            protected override Expression VisitDebugInfo(DebugInfoExpression node)
            {
                return base.VisitDebugInfo(node);
            }

            protected override Expression VisitDefault(DefaultExpression node)
            {
                return base.VisitDefault(node);
            }

            protected override Expression VisitDynamic(DynamicExpression node)
            {
                return base.VisitDynamic(node);
            }

            protected override ElementInit VisitElementInit(ElementInit node)
            {
                return base.VisitElementInit(node);
            }

            protected override Expression VisitExtension(Expression node)
            {
                return base.VisitExtension(node);
            }

            protected override Expression VisitGoto(GotoExpression node)
            {
                return base.VisitGoto(node);
            }

            protected override Expression VisitIndex(IndexExpression node)
            {
                return base.VisitIndex(node);
            }

            protected override Expression VisitInvocation(InvocationExpression node)
            {
                return base.VisitInvocation(node);
            }

            protected override Expression VisitLabel(LabelExpression node)
            {
                return base.VisitLabel(node);
            }

            protected override LabelTarget VisitLabelTarget(LabelTarget node)
            {
                return base.VisitLabelTarget(node);
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                return base.VisitLambda(node);
            }

            protected override Expression VisitLoop(LoopExpression node)
            {
                return base.VisitLoop(node);
            }

            protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
            {
                return base.VisitMemberAssignment(node);
            }

            protected override MemberBinding VisitMemberBinding(MemberBinding node)
            {
                return base.VisitMemberBinding(node);
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                return base.VisitMemberInit(node);
            }

            protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
            {
                return base.VisitMemberListBinding(node);
            }

            protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
            {
                return base.VisitMemberMemberBinding(node);
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                return base.VisitNewArray(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(node);
            }

            protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
            {
                return base.VisitRuntimeVariables(node);
            }

            protected override Expression VisitSwitch(SwitchExpression node)
            {
                return base.VisitSwitch(node);
            }

            protected override SwitchCase VisitSwitchCase(SwitchCase node)
            {
                return base.VisitSwitchCase(node);
            }

            protected override Expression VisitTry(TryExpression node)
            {
                return base.VisitTry(node);
            }

            protected override Expression VisitTypeBinary(TypeBinaryExpression node)
            {
                return base.VisitTypeBinary(node);
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                return base.VisitUnary(node);
            }
        }
    }
}