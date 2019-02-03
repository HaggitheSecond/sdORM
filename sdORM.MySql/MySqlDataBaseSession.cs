using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using sdORM.Common;
using sdORM.Common.SqlSpecifics;
using sdORM.Entities;
using sdORM.Extensions;
using sdORM.Interfaces;

namespace sdORM.MySql
{
    public class MySqlDataBaseSession : IDataBaseSession
    {
        private readonly ExpressionToMySqlProvider _expressionToMySqlProvider;

        private readonly MySqlConnection _connection;

        public EntityMappingProvider EntityMappingProvider { get; }
        public bool IsConnected => this._connection.State == ConnectionState.Open;

        public MySqlDataBaseSession(DataBaseConfig config)
        {
            this._connection = new MySqlConnection(config.ToString());

            this._expressionToMySqlProvider = new ExpressionToMySqlProvider();
            this.EntityMappingProvider = new EntityMappingProvider(this);
        }

        public void Dispose()
        {
            this._connection?.Dispose();
        }

        #region Connect

        public void Connect()
        {
            if (this.IsConnected)
                throw new Exception("Connection to database is already open!");

            this._connection.Open();
        }

        public async Task ConnectAsync()
        {
            if (this.IsConnected)
                throw new Exception("Connection to database is already open!");

            await this._connection.OpenAsync();
        }

        #endregion

        #region Query

        public async Task<IList<T>> QueryAsync<T>() where T : new()
        {
            return await this.QueryAsync<T>(new ParameterizedSql
            {
                Sql = this.GetSelectStatementForMapping(this.EntityMappingProvider.GetMapping<T>()).ToString(),
                Parameters = new List<SqlParameter>()
            });
        }

        public async Task<IList<T>> QueryAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {

            return await this.QueryAsync<T>(this.GetSqlForPredicate(predicate));
        }

        public async Task<IList<T>> QueryAsync<T>(ParameterizedSql sql) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();

            using (var command = this.GetSqlCommand(sql))
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

        public IList<T> Query<T>() where T : new()
        {
            return this.Query<T>(new ParameterizedSql
            {
                Sql = this.GetSelectStatementForMapping(this.EntityMappingProvider.GetMapping<T>()).ToString(),
                Parameters = new List<SqlParameter>()
            });
        }

        public IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return this.Query<T>(this.GetSqlForPredicate(predicate));
        }

        public IList<T> Query<T>(ParameterizedSql sql) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();

            using (var command = this.GetSqlCommand(sql))
            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows == false)
                    return default(IList<T>);

                var results = new List<T>();

                while (reader.Read())
                {
                    results.Add(reader.LoadWithEntityMapping(mapping));
                }

                return results;
            }
        }

        private ParameterizedSql GetSqlForPredicate<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();

            var wherePart = this._expressionToMySqlProvider.BuildSqlQuery(predicate);

            var builder = this.GetSelectStatementForMapping(mapping);

            if (string.IsNullOrWhiteSpace(wherePart.Sql) == false)
            {
                builder.Append(" WHERE ")
                    .Append(wherePart.Sql);
            }

            return new ParameterizedSql
            {
                Sql = builder.ToString(),
                Parameters = wherePart.Parameters
            };
        }

        #endregion

        #region GetByID

        public T GetByID<T>(object id) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.GetSqlForGetById(id, mapping);

            using (var command = this.GetSqlCommand(sql))
            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows == false)
                    return default(T);

                reader.Read();

                return reader.LoadWithEntityMapping(mapping);
            }
        }

        public async Task<T> GetByIDAsync<T>(object id) where T : new()
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.GetSqlForGetById(id, mapping);

            using (var command = this.GetSqlCommand(sql))
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows == false)
                    return default(T);

                await reader.ReadAsync();

                return reader.LoadWithEntityMapping(mapping);
            }
        }

        private ParameterizedSql GetSqlForGetById<T>(object id, EntityMapping<T> mapping)
        {
            var builder = this.GetSelectStatementForMapping(mapping)
                .Append(" WHERE ")
                .Append(mapping.PrimaryKeyPropertyMapping.ColumnName)
                .Append($" = @{mapping.PrimaryKeyPropertyMapping.ColumnName}");

            return new ParameterizedSql
            {
                Sql = builder.ToString(),
                Parameters = new List<SqlParameter>
                {
                    new SqlParameter
                    {
                        ColumnName = mapping.PrimaryKeyPropertyMapping.ColumnName,
                        ParameterName = $"@{mapping.PrimaryKeyPropertyMapping.ColumnName}",
                        Value = id
                    }
                }
            };
        }

        #endregion

        #region SaveOrUpdate

        public T SaveOrUpdate<T>(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var mapping = this.EntityMappingProvider.GetMapping<T>();

            return mapping.IsPrimaryKeyDefaultValue(entity)
                ? this.Save(entity)
                : this.Update(entity);
        }

        public Task<T> SaveOrUpdateAsync<T>(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var mapping = this.EntityMappingProvider.GetMapping<T>();

            return mapping.IsPrimaryKeyDefaultValue(entity)
                ? this.SaveAsync(entity)
                : this.UpdateAsync(entity);
        }

        #endregion

        #region Save

        public T Save<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.GetSqlForSave(entity, mapping);

            using (var command = this.GetSqlCommand(sql))
            {
                command.ExecuteNonQuery();

                this.SetIdAfterSave(entity, command, mapping);

                return entity;
            }
        }

        public async Task<T> SaveAsync<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.GetSqlForSave(entity, mapping);

            using (var command = this.GetSqlCommand(sql))
            {
                await command.ExecuteNonQueryAsync();

                this.SetIdAfterSave(entity, command, mapping);

                return entity;
            }
        }

        private void SetIdAfterSave<T>(T entity, MySqlCommand command, EntityMapping<T> mapping)
        {
            mapping.PrimaryKeyPropertyMapping
                .Property
                .SetValue(entity, Convert.ChangeType(command.LastInsertedId, mapping.PrimaryKeyPropertyMapping.Property.PropertyType));
        }

        private ParameterizedSql GetSqlForSave<T>(T entity, EntityMapping<T> mapping)
        {
            if (mapping.IsPrimaryKeyDefaultValue(entity) == false)
                throw new InvalidOperationException($"Entity of type {entity.GetType()} and id {mapping.GetPrimaryKeyValue(entity)} is already saved. Please use SaveOrUpdate or Update.");

            var parameters = this.GetParametersForMapping(entity, mapping).ToList();

            var builder = new StringBuilder()
                .Append("INSERT INTO ")
                .Append(mapping.TableName)
                .Append(" (")
                .AppendJoin(", ", mapping.Properties.Select(f => f.ColumnName))
                .Append(") VALUES ( ")
                .AppendJoin(", ", parameters.Select(f => f.ParameterName))
                .Append(");");

            return new ParameterizedSql
            {
                Sql = builder.ToString(),
                Parameters = parameters
            };
        }

        #endregion

        #region Update

        public T Update<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.GetSqlForUpdate(entity, mapping);

            using (var command = this.GetSqlCommand(sql))
            {
                command.ExecuteNonQuery();
                return entity;
            }
        }

        public async Task<T> UpdateAsync<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.GetSqlForUpdate(entity, mapping);

            using (var command = this.GetSqlCommand(sql))
            {
                await command.ExecuteNonQueryAsync();
                return entity;
            }
        }

        private ParameterizedSql GetSqlForUpdate<T>(T entity, EntityMapping<T> mapping)
        {

            if (mapping.IsPrimaryKeyDefaultValue(entity))
                throw new InvalidOperationException($"Entity of type {entity.GetType()} is not yet saved and cannot be updated. Please use SaveOrUpdate or Save.");

            var parameters = this.GetParametersForMapping(entity, mapping).ToList();

            var builder = new StringBuilder()
                .Append("UPDATE ")
                .Append(mapping.TableName)
                .Append(" SET ")
                .AppendJoin(", ", parameters.Select(f => $"{f.ColumnName} = {f.ParameterName}").ToList())
                .Append(" WHERE ")
                .Append($" {mapping.PrimaryKeyPropertyMapping.ColumnName} = {mapping.GetPrimaryKeyValue(entity)}");

            return new ParameterizedSql
            {
                Sql = builder.ToString(),
                Parameters = parameters
            };
        }

        #endregion

        #region MetaData

        public TableMetaData GetTableMetaData(string tableName)
        {
            // I'm not sure if returning null if it doesnt exist is really what we want to do here.
            // Throwing an exception might be the better option but simply returning null is consitent with how the database does it. Not sure...
            using (var cmd = this.GetSqlCommand(this.GetSqlForCheckIfTableExtists(tableName)))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows == false)
                    return null;
            }

            using (var cmd = this.GetSqlCommand(this.GetSqlForTableMetaData(tableName)))
            using (var reader = cmd.ExecuteReader())
            {
                var table = new TableMetaData
                {
                    Columns = new List<ColumnMetaData>()
                };

                while (reader.Read())
                {
                    table.Columns.Add(reader.LoadColumnMetaData());
                }

                return table;
            }
        }

        public async Task<TableMetaData> GetTableMetaDataAsync(string tableName)
        {
            // See sync version for comment
            using (var cmd = this.GetSqlCommand(this.GetSqlForCheckIfTableExtists(tableName)))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows == false)
                    return null;
            }

            using (var cmd = this.GetSqlCommand(this.GetSqlForTableMetaData(tableName)))
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

        private string GetSqlForTableMetaData(string tableName)
        {
            return $"SELECT column_name, ordinal_position, data_type, column_type FROM information_schema.columns WHERE table_name = '{tableName}'";
        }

        private string GetSqlForCheckIfTableExtists(string tableName)
        {
            return $"SHOW TABLES LIKE '{tableName}'";
        }

        #endregion

        #region GenericSql

        private StringBuilder GetSelectStatementForMapping<T>(EntityMapping<T> mapping)
        {
            return new StringBuilder()
                .Append("SELECT ")
                .Append($"{mapping.PrimaryKeyPropertyMapping.ColumnName}, ")
                .AppendJoin(", ", mapping.Properties.Select(f => f.ColumnName))
                .Append(" FROM ")
                .Append(mapping.TableName);
        }

        private IEnumerable<SqlParameter> GetParametersForMapping<T>(T entity, EntityMapping<T> mapping)
        {
            return mapping.Properties.Select(currentProperty => new SqlParameter
            {
                ColumnName = currentProperty.ColumnName,
                ParameterName = $"@{currentProperty.ColumnName}",
                Value = currentProperty.Property.GetValue(entity)
            });
        }

        #endregion

        #region Command

        private MySqlCommand GetSqlCommand(ParameterizedSql sql)
        {
            var cmd = this.GetSqlCommand(sql.Sql);

            foreach (var currentParameter in sql.Parameters)
            {
                cmd.Parameters.AddWithValue(currentParameter.ParameterName, currentParameter.Value);
            }

            return cmd;
        }

        private MySqlCommand GetSqlCommand(string sql)
        {
            return new MySqlCommand(sql)
            {
                Connection = this._connection
            };
        }

        #endregion

    }
}