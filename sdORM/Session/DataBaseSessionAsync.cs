using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using sdORM.Common;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Helper;
using sdORM.Mapping;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace sdORM.Session
{
    public class DatabaseSessionAsync : DatabaseSessionBase, IDatabaseSessionAsync
    {
        public DatabaseSessionAsync(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecifcProvider sqlSpecifcProvider)
            : base(connection, entityMappingProvider, sqlSpecifcProvider)
        {

        }

        public virtual async Task ConnectAsync()
        {
            await this.Connection.OpenAsync();
        }

        public virtual async Task<IList<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return await this.QueryAsync<T>(this.SqlSpecifcProvider.GetSqlForPredicate(predicate, this.EntityMappingProvider.GetMapping<T>(), this.SqlSpecifcProvider.ExpressionToSqlProvider));
        }

        public virtual async Task<IList<T>> QueryAsync<T>() where T : new()
        {
            return await this.QueryAsync<T>(new ParameterizedSql
            {
                Sql = this.SqlSpecifcProvider.GetSelectStatementForMapping(this.EntityMappingProvider.GetMapping<T>()).ToString(),
                Parameters = new List<SqlParameter>()
            });
        }

        public virtual async Task<IList<T>> QueryAsync<T>(ParameterizedSql sql) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();

            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, sql))
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows == false)
                    return default(IList<T>);

                var results = new List<T>();

                while (await reader.ReadAsync())
                {
                    results.Add(reader.LoadWithEntityMapping(mapping));
                }

                return results;
            }
        }

        public virtual async Task<T> GetByIDAsync<T>(object id) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecifcProvider.GetSqlForGetById(id, mapping);

            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, sql))
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows == false)
                    return default(T);

                await reader.ReadAsync();

                return reader.LoadWithEntityMapping(mapping);
            }
        }

        public virtual Task<T> SaveOrUpdateAsync<T>(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var mapping = this.EntityMappingProvider.GetMapping<T>();

            return mapping.IsPrimaryKeyDefaultValue(entity)
                ? this.SaveAsync(entity)
                : this.UpdateAsync(entity);
        }

        public virtual async Task<T> SaveAsync<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecifcProvider.GetSqlForSave(entity, mapping);

            using (var transaction = this.Connection.BeginTransaction())
            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, sql, transaction))
            {
                await command.ExecuteNonQueryAsync();

                transaction.Commit();
                this.SqlSpecifcProvider.SetIdAfterSave(entity, command, mapping);

                return entity;
            }
        }

        public virtual async Task<T> UpdateAsync<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecifcProvider.GetSqlForUpdate(entity, mapping);

            using (var transaction = this.Connection.BeginTransaction())
            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, sql, transaction))
            {
                await command.ExecuteNonQueryAsync();
                transaction.Commit();
                return entity;
            }
        }

        public virtual async Task Delete<T>(object id)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecifcProvider.GetSqlForDelete(id, mapping);

            using (var transaction = this.Connection.BeginTransaction())
            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, sql, transaction))
            {
                await command.ExecuteNonQueryAsync();
                transaction.Commit();
            }
        }

        public virtual async Task<TableMetaData> GetTableMetaDataAsync(string tableName)
        {
            // See sync version for comment
            using (var cmd = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, this.SqlSpecifcProvider.GetSqlForCheckIfTableExtists(tableName)))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows == false)
                    return null;
            }

            using (var cmd = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, this.SqlSpecifcProvider.GetSqlForTableMetaData(tableName)))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                var table = new TableMetaData
                {
                    Columns = new List<ColumnMetaData>()
                };

                while (await reader.ReadAsync())
                {
                    table.Columns.Add(reader.LoadColumnMetaData());
                }

                return table;
            }
        }
    }
}