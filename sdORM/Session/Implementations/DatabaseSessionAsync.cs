using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Helper;
using sdORM.Mapping;
using sdORM.Session.Interfaces;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace sdORM.Session.Implementations
{
    public class DatabaseSessionAsync : DatabaseSessionBase, IDatabaseSessionAsync
    {
        public DatabaseSessionAsync(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecificProvider sqlSpecificProvider)
            : base(connection, entityMappingProvider, sqlSpecificProvider)
        {

        }

        public virtual async Task<IList<T>> QueryAsync<T>() where T : new()
        {
            return await this.QueryAsyncInternal<T>(this.SqlSpecificProvider.GetSelectStatementForMapping(this.EntityMappingProvider.GetMapping<T>()).ToString());
        }

        public virtual async Task<IList<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return await this.QueryAsyncInternal<T>(this.SqlSpecificProvider.GetSqlForPredicate(predicate, this.EntityMappingProvider.GetMapping<T>()));
        }
        
        private async Task<IList<T>> QueryAsyncInternal<T>(ParameterizedSql sql) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();

            using (var command = this.GenerateCommand(sql))
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
            var sql = this.SqlSpecificProvider.GetSqlForGetById(id, mapping);

            using (var command = this.GenerateCommand(sql))
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
            var sql = this.SqlSpecificProvider.GetSqlForSave(entity, mapping);

            using (var command = this.GenerateCommand(sql))
            {
                await command.ExecuteNonQueryAsync();

                this.SqlSpecificProvider.SetIdAfterSave(entity, command, mapping);

                return entity;
            }
        }

        public virtual async Task<T> UpdateAsync<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecificProvider.GetSqlForUpdate(entity, mapping);

            using (var command = this.GenerateCommand(sql))
            {
                await command.ExecuteNonQueryAsync();
                return entity;
            }
        }

        public virtual async Task Delete<T>(object id)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecificProvider.GetSqlForDelete(id, mapping);

            using (var command = this.GenerateCommand(sql))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        public virtual async Task<TableMetaData> GetTableMetaDataAsync(string tableName)
        {
            // See sync version for comment
            using (var cmd = this.GenerateCommand(this.SqlSpecificProvider.GetSqlForCheckIfTableExtists(tableName)))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows == false)
                    return null;
            }

            using (var cmd = this.GenerateCommand(this.SqlSpecificProvider.GetSqlForTableMetaData(tableName)))
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