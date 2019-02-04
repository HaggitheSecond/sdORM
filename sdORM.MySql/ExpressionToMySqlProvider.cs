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
            var sqlScriptParts = new List<SqlScriptPart>();

            this.ProcessExpression(expression.Body, sqlScriptParts);

            var result = new ParameterizedSql
            {
                Sql = this.ToRawSql(sqlScriptParts),
                Parameters = new List<SqlParameter>(sqlScriptParts
                    .OfType<SqlScriptParameter>()
                    .Select(f => new SqlParameter
                    {
                        Value = f.ParameterValue,
                        ParameterName = f.ParameterName
                    }))
            };

            return result;
        }

        private string ToRawSql(IList<SqlScriptPart> scriptParts)
        {
            var result = new List<string>();

            foreach (var currentScriptPart in scriptParts)
            {
                if (currentScriptPart is SqlScriptColumnName columnName)
                    result.Add(columnName.ColumnName);

                if (currentScriptPart is SqlScriptOperator @operator)
                    result.Add(@operator.Value);

                if (currentScriptPart is SqlScriptParameter parameter)
                    result.Add(parameter.ParameterName);

            }

            return string.Join(" ", result);
        }

        private void ProcessExpression(Expression expression, IList<SqlScriptPart> sqlScriptParts)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                this.ProcessExpression(binaryExpression.Left, sqlScriptParts);
                sqlScriptParts.Add(new SqlScriptOperator
                {
                    Value = binaryExpression.NodeType.ToSqlOperator()
                });
                this.ProcessExpression(binaryExpression.Right, sqlScriptParts);
            }
            else if (expression is InvocationExpression invocationExpression)
            {
                this.ProcessExpression(invocationExpression.Expression, sqlScriptParts);
            }
            else if (expression is LambdaExpression lambdaExpression)
            {
                if (lambdaExpression.Body is BinaryExpression binaryExpressionBody && (binaryExpressionBody.NodeType == ExpressionType.AndAlso ||
                                                                                      binaryExpressionBody.NodeType == ExpressionType.OrElse))
                {
                    sqlScriptParts.Add(new SqlScriptOperator
                    {
                        Value = "("
                    });

                    this.ProcessExpression(lambdaExpression.Body, sqlScriptParts);

                    sqlScriptParts.Add(new SqlScriptOperator
                    {
                        Value = ")"
                    });
                }
                else
                    this.ProcessExpression(lambdaExpression.Body, sqlScriptParts);
            }
            else if (expression is MemberExpression memberExpression)
            {
                sqlScriptParts.Add(new SqlScriptColumnName
                {
                    ColumnName = $"{memberExpression.Member.Name}"
                });
            }
            else if (expression is ConstantExpression constantExpression)
            {
                if (sqlScriptParts.LastOrDefault(f => f is SqlScriptColumnName) is SqlScriptColumnName sqlPart)
                    sqlScriptParts.Add(new SqlScriptParameter
                    {
                        ParameterValue = constantExpression.Value,
                        ParameterName = this.GetUnusedParameterName($"@{sqlPart.ColumnName}", sqlScriptParts.OfType<SqlScriptParameter>().ToList())
                    });
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Object == null)
                    throw new ArgumentException("MethodCallExpression.Object is null", nameof(methodCallExpression.Object));

                var type = methodCallExpression.Object.Type;

                if (type == typeof(string))
                {
                    this.HandleStringOperations(methodCallExpression, sqlScriptParts);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    this.HandleListOperations(methodCallExpression, sqlScriptParts);
                }
            }
            else
            {
                throw new Exception($"Expression of type {expression.Type} is not allowed in this context");
            }
        }

        private void HandleListOperations(MethodCallExpression expression, IList<SqlScriptPart> sqlScriptParts)
        {
            if (expression.Method.Name == "Contains")
            {
                var columnName = ((MemberExpression)expression.Arguments[0]).Member.Name;

                sqlScriptParts.Add(new SqlScriptColumnName
                {
                    ColumnName = columnName
                });

                sqlScriptParts.Add(new SqlScriptOperator
                {
                    Value = "IN"
                });

                var values = (IList)Expression.Lambda((MemberExpression)expression.Object).Compile().DynamicInvoke();

                sqlScriptParts.Add(new SqlScriptOperator
                {
                    Value = "("
                });

                foreach (var currentValue in values)
                {
                    sqlScriptParts.Add(new SqlScriptParameter
                    {
                        ParameterValue = currentValue,
                        ParameterName = this.GetUnusedParameterName($"@{columnName}", sqlScriptParts.OfType<SqlScriptParameter>().ToList())
                    });

                    if (values.IndexOf(currentValue) != values.Count - 1)
                        sqlScriptParts.Add(new SqlScriptOperator
                        {
                            Value = ","
                        });
                }

                sqlScriptParts.Add(new SqlScriptOperator
                {
                    Value = ")"
                });
            }
        }

        private void HandleStringOperations(MethodCallExpression expression, IList<SqlScriptPart> sqlScriptParts)
        {
            if (expression.Method.Name == "Contains")
            {
                var columnName = ((MemberExpression)expression.Object).Member.Name;

                sqlScriptParts.Add(new SqlScriptColumnName
                {
                    ColumnName = columnName
                });

                sqlScriptParts.Add(new SqlScriptOperator
                {
                    Value = "LIKE"
                });

                string parameterValue;

                switch (expression.Arguments.First())
                {
                    case ConstantExpression constantArgumentExpression:
                        parameterValue = constantArgumentExpression.Value.ToString();
                        break;
                    case MethodCallExpression methodCallExpression:
                        parameterValue = Expression.Lambda(methodCallExpression).Compile().DynamicInvoke().ToString();
                        break;
                    default:
                        throw new ArgumentException();
                }

                sqlScriptParts.Add(new SqlScriptParameter
                {
                    ParameterName = this.GetUnusedParameterName($"@{columnName}", sqlScriptParts.OfType<SqlScriptParameter>().ToList()),
                    ParameterValue = $"%{parameterValue}%"
                });
            }
        }

        private string GetUnusedParameterName(string preferedName, IList<SqlScriptParameter> existingParameters)
        {
            if (existingParameters.Select(f => f.ParameterName).Contains(preferedName) == false)
                return preferedName;

            var newName = string.Empty;
            var i = 1;

            while (true)
            {
                newName = preferedName + i;

                if (existingParameters.Select(f => f.ParameterName).Contains(newName) == false)
                    break;

                i++;
            }

            return newName;
        }

        private class SqlScriptColumnName : SqlScriptPart
        {
            public string ColumnName { get; set; }
        }

        private class SqlScriptParameter : SqlScriptPart
        {
            public object ParameterValue { get; set; }

            public string ParameterName { get; set; }
        }

        private class SqlScriptOperator : SqlScriptPart
        {
            public string Value { get; set; }
        }

        private abstract class SqlScriptPart
        {
        }
    }
}