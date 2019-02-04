using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using sdORM.Common;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Helper;
using sdORM.Mapping;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace sdORM.Session
{
    public class DatabaseSession : DatabaseSessionBase, IDatabaseSession
    {
        public DatabaseSession(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecifcProvider sqlSpecifcProvider)
            : base(connection, entityMappingProvider, sqlSpecifcProvider)
        {

        }

        public virtual void Connect()
        {
            this.Connection.Open();
        }

        public virtual IList<T> Query<T>() where T : new()
        {
            return this.Query<T>(new ParameterizedSql
            {
                Sql = this.SqlSpecifcProvider.GetSelectStatementForMapping(this.EntityMappingProvider.GetMapping<T>()).ToString(),
                Parameters = new List<SqlParameter>()
            });
        }

        public virtual IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            Guard.NotNull(predicate, nameof(predicate));

            return this.Query<T>(this.SqlSpecifcProvider.GetSqlForPredicate(predicate, this.EntityMappingProvider.GetMapping<T>(), this.SqlSpecifcProvider.ExpressionToSqlProvider));
        }

        public virtual IList<T> Query<T>(ParameterizedSql parameterizedSql) where T : new()
        {
            Guard.NotNull(parameterizedSql, nameof(parameterizedSql));

            var mapping = this.EntityMappingProvider.GetMapping<T>();

            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, parameterizedSql))
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

        public virtual T GetByID<T>(object id) where T : new()
        {
            Guard.NotNull(id, nameof(id));

            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecifcProvider.GetSqlForGetById(id, mapping);

            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, sql))
            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows == false)
                    return default(T);

                reader.Read();

                return reader.LoadWithEntityMapping(mapping);
            }
        }

        public virtual T SaveOrUpdate<T>(T entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var mapping = this.EntityMappingProvider.GetMapping<T>();

            return mapping.IsPrimaryKeyDefaultValue(entity)
                ? this.Save(entity)
                : this.Update(entity);
        }

        public virtual T Save<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecifcProvider.GetSqlForSave(entity, mapping);

            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, sql))
            {
                command.ExecuteNonQuery();

                this.SqlSpecifcProvider.SetIdAfterSave(entity, command, mapping);

                return entity;
            }
        }

        public virtual T Update<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecifcProvider.GetSqlForUpdate(entity, mapping);

            using (var command = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, sql))
            {
                command.ExecuteNonQuery();
                return entity;
            }
        }

        public virtual TableMetaData GetTableMetaData(string tableName)
        {
            // I'm not sure if returning null if it doesnt exist is really what we want to do here.
            // Throwing an exception might be the better option but simply returning null is consitent with how the Database does it. Not sure...
            using (var cmd = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, this.SqlSpecifcProvider.GetSqlForCheckIfTableExtists(tableName)))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows == false)
                    return null;
            }

            using (var cmd = this.SqlSpecifcProvider.GenerateIDBCommand(this.Connection, this.SqlSpecifcProvider.GetSqlForTableMetaData(tableName)))
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
    }
}