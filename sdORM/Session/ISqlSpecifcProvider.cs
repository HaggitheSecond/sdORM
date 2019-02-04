using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using sdORM.Common.SqlSpecifics;
using sdORM.Mapping;

namespace sdORM.Session
{
    public interface ISqlSpecifcProvider
    {
        IExpressionToSqlProvider ExpressionToSqlProvider { get; }

        ParameterizedSql GetSqlForPredicate<T>(Expression<Func<T, bool>> predicate, EntityMapping<T> mapping,IExpressionToSqlProvider provider) where T : new();

        ParameterizedSql GetSqlForGetById<T>(object id, EntityMapping<T> mapping);

        void SetIdAfterSave<T>(T entity, IDbCommand command, EntityMapping<T> mapping);
        ParameterizedSql GetSqlForSave<T>(T entity, EntityMapping<T> mapping);

        ParameterizedSql GetSqlForUpdate<T>(T entity, EntityMapping<T> mapping);

        string GetSqlForTableMetaData(string tableName);
        string GetSqlForCheckIfTableExtists(string tableName);
        
        StringBuilder GetSelectStatementForMapping<T>(EntityMapping<T> mapping);
        IEnumerable<SqlParameter> GetParametersForMapping<T>(T entity, EntityMapping<T> mapping);

        DbCommand GenerateIDBCommand(DbConnection connection, ParameterizedSql sql);
        DbCommand GenerateIDBCommand(DbConnection connection, string sql);
    }
}