using System;
using System.Linq.Expressions;

namespace sdORM.Common.Queries
{
    public class ExpressionBuilder<T>
    {
        private Expression<Func<T, bool>> _expression;

        public ExpressionBuilder(Expression<Func<T, bool>> func)
        {
            this._expression = func;
        }

        public ExpressionBuilder<T> And(Expression<Func<T, bool>> func)
        {
            var wokeExpression = Expression.Invoke(func, this._expression.Parameters);

            this._expression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(this._expression.Body, wokeExpression), this._expression.Parameters);

            return this;
        }

        public ExpressionBuilder<T> Or(Expression<Func<T, bool>> func)
        {
            var wokeExpression = Expression.Invoke(func, this._expression.Parameters);

            this._expression = Expression.Lambda<Func<T, bool>>(Expression.OrElse(this._expression.Body, wokeExpression), this._expression.Parameters);

            return this;
        }

        public Expression<Func<T, bool>> Create()
        {
            return this._expression;
        }

        public static implicit operator Expression<Func<T, bool>>(ExpressionBuilder<T> builder)
        {
            return builder.Create();
        }
    }
}