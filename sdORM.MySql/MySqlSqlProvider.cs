using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MySql.Data.MySqlClient;
using sdORM.Common.SqlSpecifics;
using sdORM.Mapping;
using sdORM.Session;

namespace sdORM.MySql
{
    public class MySqlSqlProvider : ISqlSpecifcProvider
    {
        public IExpressionToSqlProvider ExpressionToSqlProvider => new ExpressionToMySqlProvider();

        public ParameterizedSql GetSqlForPredicate<T>(Expression<Func<T, bool>> predicate, EntityMapping<T> mapping, IExpressionToSqlProvider provider) where T : new()
        {
            var wherePart = provider.BuildSqlQuery(predicate);

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

        public ParameterizedSql GetSqlForGetById<T>(object id, EntityMapping<T> mapping)
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
        
        public void SetIdAfterSave<T>(T entity, IDbCommand command, EntityMapping<T> mapping)
        {
            var mySqlCommand = (MySqlCommand)command;

            mapping.PrimaryKeyPropertyMapping
                .Property
                .SetValue(entity, Convert.ChangeType(mySqlCommand.LastInsertedId, mapping.PrimaryKeyPropertyMapping.Property.PropertyType));
        }

        public ParameterizedSql GetSqlForSave<T>(T entity, EntityMapping<T> mapping)
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
        
        public ParameterizedSql GetSqlForUpdate<T>(T entity, EntityMapping<T> mapping)
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
        
        public string GetSqlForTableMetaData(string tableName)
        {
            return $"SELECT column_name, ordinal_position, data_type, column_type FROM information_schema.columns WHERE table_name = '{tableName}'";
        }

        public string GetSqlForCheckIfTableExtists(string tableName)
        {
            return $"SHOW TABLES LIKE '{tableName}'";
        }

        public DbCommand GenerateIDBCommand(DbConnection connection, ParameterizedSql sql)
        {
            var command = this.GenerateIDBCommand(connection, sql.Sql);

            foreach (var currentParameter in sql.Parameters)
            {
                var parameter = command.CreateParameter();

                parameter.ParameterName = currentParameter.ParameterName;
                parameter.Value = currentParameter.Value;

                command.Parameters.Add(parameter);
            }

            return command;
        }

        public DbCommand GenerateIDBCommand(DbConnection connection, string sql)
        {
            var command = connection.CreateCommand();

            command.CommandText = sql;

            return command;
        }

        public StringBuilder GetSelectStatementForMapping<T>(EntityMapping<T> mapping)
        {
            return new StringBuilder()
                .Append("SELECT ")
                .Append($"{mapping.PrimaryKeyPropertyMapping.ColumnName}, ")
                .AppendJoin(", ", mapping.Properties.Select(f => f.ColumnName))
                .Append(" FROM ")
                .Append(mapping.TableName);
        }

        public IEnumerable<SqlParameter> GetParametersForMapping<T>(T entity, EntityMapping<T> mapping)
        {
            return mapping.Properties.Select(currentProperty => new SqlParameter
            {
                ColumnName = currentProperty.ColumnName,
                ParameterName = $"@{currentProperty.ColumnName}",
                Value = currentProperty.Property.GetValue(entity)
            });
        }
    }
}