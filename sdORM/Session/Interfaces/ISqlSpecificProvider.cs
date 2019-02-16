using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using sdORM.Common.SqlSpecifics;
using sdORM.Mapping;

namespace sdORM.Session.Interfaces
{
    public interface ISqlSpecificProvider
    {
        IExpressionToSqlProvider ExpressionToSqlProvider { get; }

        ParameterizedSql GetSqlForPredicate<T>(Expression<Func<T, bool>> predicate, EntityMapping<T> mapping) where T : new();

        ParameterizedSql GetSqlForGetById<T>(object id, EntityMapping<T> mapping);

        void SetIdAfterSave<T>(T entity, IDbCommand command, EntityMapping<T> mapping);
        ParameterizedSql GetSqlForSave<T>(T entity, EntityMapping<T> mapping);

        ParameterizedSql GetSqlForUpdate<T>(T entity, EntityMapping<T> mapping);

        ParameterizedSql GetSqlForDelete<T>(object id, EntityMapping<T> mapping);

        string GetSqlForTableMetaData(string tableName);
        string GetSqlForCheckIfTableExtists(string tableName);
        
        StringBuilder GetSelectStatementForMapping<T>(EntityMapping<T> mapping);
        IEnumerable<SqlParameter> GetParametersForMapping<T>(T entity, EntityMapping<T> mapping);
    }
}