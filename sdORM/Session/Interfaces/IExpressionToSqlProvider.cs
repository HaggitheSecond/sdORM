using System;
using System.Linq.Expressions;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Session.Interfaces
{
    public interface IExpressionToSqlProvider
    {
        ParameterizedSql BuildSqlQuery<T>(Expression<Func<T, bool>> expression);
    }
}