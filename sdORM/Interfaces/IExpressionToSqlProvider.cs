using System;
using System.Data.Common;
using System.Linq.Expressions;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Interfaces
{
    public interface IExpressionToSqlProvider
    {
        ParameterizedSql BuildSqlWhereQuery<T>(Expression<Func<T, bool>> expression);
    }
}