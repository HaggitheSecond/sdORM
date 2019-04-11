using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;
using sdORM.Mapping;

namespace sdORM.Session.Interfaces
{
    public interface ISqlSpecificProvider
    {
        IExpressionToSqlProvider ExpressionToSqlProvider { get; }

        ParameterizedSql GetSqlForPredicate<T>(Expression<Func<T, bool>> predicate, EntityMapping<T> mapping) where T : new();

        ParameterizedSql GetSqlForGetById<T>(object id, EntityMapping<T> mapping);

        //TODO: make this better to use - currenlty only work nicely for mysql 
        void ExecuteSaveCommandAndSetPrimeryKeyProperty<T>(T entity, IDbCommand command, EntityMapping<T> mapping);
        Task ExecuteSaveCommandAndSetPrimeryKeyPropertyAsync<T>(T entity, DbCommand command, EntityMapping<T> mapping);
        ParameterizedSql GetSqlForSave<T>(T entity, EntityMapping<T> mapping);

        ParameterizedSql GetSqlForUpdate<T>(T entity, EntityMapping<T> mapping);

        ParameterizedSql GetSqlForDelete<T>(object id, EntityMapping<T> mapping);

        string GetSqlForTableMetaData(string tableName);
        string GetSqlForCheckIfTableExists(string tableName);
        
        StringBuilder GetSelectStatementForMapping<T>(EntityMapping<T> mapping);
        IEnumerable<SqlParameter> GetParametersForMapping<T>(T entity, EntityMapping<T> mapping);
    }
}