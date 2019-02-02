using System;
using System.Linq.Expressions;

namespace sdORM.Extensions
{
    public static class ExpressionTypeExtensions
    {
        public static string ToSqlOperator(this ExpressionType self)
        {
            switch (self)
            {
                case ExpressionType.Add:
                    return "+";

                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "AND";

                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.ExclusiveOr:
                    return "^";

                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";

                case ExpressionType.Negate:
                    return "-";
                case ExpressionType.Not:
                    return "NOT";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return "OR";
                default:
                    throw new Exception($"ExpressionType {self} is not supported");
            }
        }
    }
}