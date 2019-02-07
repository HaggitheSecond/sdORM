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
                    this._ignoreNextMemberCount--;
                else
                    this._expressionParts.Add(node.Member.Name);

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
        }
    }
}