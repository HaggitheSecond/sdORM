using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Helper;
using sdORM.Mapping;
using sdORM.Session.Interfaces;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace sdORM.Session.Implementations
{
    public class DatabaseSession : DatabaseSessionBase, IDatabaseSession
    {
        public DatabaseSession(DbConnection connection, EntityMappingProvider entityMappingProvider, ISqlSpecificProvider sqlSpecificProvider)
            : base(connection, entityMappingProvider, sqlSpecificProvider)
        {

        }

        public virtual IList<T> Query<T>() where T : new()
        {
            return this.QueryInternal<T>(this.SqlSpecificProvider.GetSelectStatementForMapping(this.EntityMappingProvider.GetMapping<T>()).ToString());
        }

        public virtual IList<T> Query<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            Guard.NotNull(predicate, nameof(predicate));

            return this.QueryInternal<T>(this.SqlSpecificProvider.GetSqlForPredicate(predicate, this.EntityMappingProvider.GetMapping<T>()));
        }

        private IList<T> QueryInternal<T>(ParameterizedSql sql) where T : new()
        {
            Guard.NotNull(sql, nameof(sql));

            var mapping = this.EntityMappingProvider.GetMapping<T>();

            using (var command = this.GenerateCommand(sql))
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
            var sql = this.SqlSpecificProvider.GetSqlForGetById(id, mapping);

            using (var command = this.GenerateCommand(sql))
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
            var sql = this.SqlSpecificProvider.GetSqlForSave(entity, mapping);

            using (var command = this.GenerateCommand(sql))
            {
                this.SqlSpecificProvider.ExecuteSaveCommandAndSetPrimeryKeyProperty(entity, command, mapping);
                
                return entity;
            }
        }

        public virtual T Update<T>(T entity)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecificProvider.GetSqlForUpdate(entity, mapping);

            using (var command = this.GenerateCommand(sql))
            {
                command.ExecuteNonQuery();
                return entity;
            }
        }

        public virtual void Delete<T>(object id)
        {
            var mapping = this.EntityMappingProvider.GetMapping<T>();
            var sql = this.SqlSpecificProvider.GetSqlForDelete(id, mapping);

            using (var command = this.GenerateCommand(sql))
            {
                command.ExecuteNonQuery();
            }
        }

        public virtual TableMetaData GetTableMetaData(string tableName)
        {
            // I'm not sure if returning null if it doesnt exist is really what we want to do here.
            // Throwing an exception might be the better option but simply returning null is consitent with how the Database does it. Not sure...
            using (var cmd = this.GenerateCommand(this.SqlSpecificProvider.GetSqlForCheckIfTableExists(tableName)))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows == false)
                    return null;
            }

            using (var cmd = this.GenerateCommand(this.SqlSpecificProvider.GetSqlForTableMetaData(tableName)))
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