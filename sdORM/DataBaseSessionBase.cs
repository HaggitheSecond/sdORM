using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using sdORM.Common;
using sdORM.Common.SqlSpecifics;
using sdORM.Entities;

namespace sdORM
{
    public abstract class DataBaseSessionBase
    {
        protected readonly DbConnection _connection;

        protected EntityMappingProvider EntityMappingProvider { get; }

        protected DataBaseSessionBase(DbConnection connection, EntityMappingProvider entityMappingProvider)
        {
            Guard.NotNull(connection, nameof(connection));
            Guard.NotNull(entityMappingProvider, nameof(entityMappingProvider));

            this._connection = connection;
            this.EntityMappingProvider = entityMappingProvider;
        }

        public void Dispose()
        {
            this._connection?.Dispose();
        }

        protected abstract ParameterizedSql GetSqlForPredicate<T>(Expression<Func<T, bool>> predicate, EntityMapping<T> mapping) where T : new();

        protected abstract ParameterizedSql GetSqlForGetById<T>(object id, EntityMapping<T> mapping);

        protected abstract void SetIdAfterSave<T>(T entity, IDbCommand command, EntityMapping<T> mapping);
        protected abstract ParameterizedSql GetSqlForSave<T>(T entity, EntityMapping<T> mapping);

        protected abstract ParameterizedSql GetSqlForUpdate<T>(T entity, EntityMapping<T> mapping);

        protected abstract string GetSqlForTableMetaData(string tableName);
        protected abstract string GetSqlForCheckIfTableExtists(string tableName);
        
        protected virtual DbCommand GenerateIDBCommand(ParameterizedSql sql)
        {
            var command = this.GenerateIDBCommand(sql.Sql);

            foreach (var currentParameter in sql.Parameters)
            {
                var parameter = command.CreateParameter();

                parameter.ParameterName = currentParameter.ParameterName;
                parameter.Value = currentParameter.Value;

                command.Parameters.Add(parameter);
            }

            return command;
        }

        protected abstract DbCommand GenerateIDBCommand(string sql);

        protected abstract StringBuilder GetSelectStatementForMapping<T>(EntityMapping<T> mapping);
        protected abstract IEnumerable<SqlParameter> GetParametersForMapping<T>(T entity, EntityMapping<T> mapping);
    }
}