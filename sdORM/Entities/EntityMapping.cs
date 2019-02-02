using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using sdORM.Attributes;
using sdORM.Attributes.Entities;
using sdORM.Attributes.Properties;
using sdORM.Common.SqlSpecifics;
using sdORM.Exceptions;
using sdORM.Extensions;
using sdORM.Interfaces;

namespace sdORM.Entities
{
    public abstract class EntityMapping
    {
        protected readonly IDataBaseSession _session;

        protected EntityMapping(IDataBaseSession session)
        {
            this._session = session;
        }

        public abstract void Map();

        public abstract Task MapAsync();
    }

    public class EntityMapping<T> : EntityMapping
    {
        public string TableName { get; private set; }
        public DBPropertyMapping PrimaryKeyPropertyMapping { get; private set; }
        public IList<DBPropertyMapping> Properties { get; private set; }

        public EntityMapping(IDataBaseSession session)
            : base(session)
        {
        }

        public override void Map()
        {
            this.ValidateEntity();
            this.Load();
            this.ValidateAfterLoad();

            this.ValidateAgainstDatabase();
        }

        public override async Task MapAsync()
        {
            this.ValidateEntity();
            this.Load();
            this.ValidateAfterLoad();

            await this.ValidateAgainstDatabaseAsync();
        }

        #region Validation

        private void ValidateEntity()
        {
            var type = typeof(T);

            if (type.GetCustomAttribute<DBEntity>() == null)
                throw new NoDBEntityMappingException(typeof(T));

            this.ValidateType(type);
        }

        private void ValidateType(Type type)
        {
            foreach (var currentProperty in type.GetProperties())
            {
                this.ValidateProperty(currentProperty);
            }

            if (type.BaseType != null)
                this.ValidateType(type.BaseType);
        }

        private void ValidateProperty(PropertyInfo property)
        {
            if (property.GetCustomAttribute<DBProperty>() == null
                && property.GetCustomAttribute<DBIgnore>() == null
                && property.GetCustomAttribute<DBPrimaryKey>() == null)
                throw new NoDBPropertyMappingException(property.DeclaringType, property);

            if (property.GetCustomAttribute<DBPrimaryKey>() != null)
            {
                if (property.PropertyType.IsValueType == false || property.PropertyType.IsNullable())
                    throw new BadDBPropertyMappingException(property.DeclaringType, property, "The primary key must be a non-nullable value type.");

                if (property.PropertyType != typeof(int))
                    throw new BadDBPropertyMappingException(property.DeclaringType, property, $"'{property.PropertyType}' is not supported as a primary key.");
            }
        }

        private void ValidateAfterLoad()
        {
            if (this.PrimaryKeyPropertyMapping == null)
                throw new NoPrimaryKeyMappingForDBEntityException(typeof(T));
        }

        private void ValidateAgainstDatabase()
        {
            this.ValidateAgainstTableMetaData(this._session.GetTableMetaData(this.TableName));
        }

        private async Task ValidateAgainstDatabaseAsync()
        {
            this.ValidateAgainstTableMetaData(await this._session.GetTableMetaDataAsync(this.TableName));
        }

        private void ValidateAgainstTableMetaData(TableMetaData tableMetadata)
        {
            if (tableMetadata.Columns.Any(f => f.ColumnName == this.PrimaryKeyPropertyMapping.ColumnName) == false)
                throw new NoMatchingColumnForDBPropertyException(this.TableName, this.PrimaryKeyPropertyMapping.ColumnName);

            foreach (var currentProperty in this.Properties)
            {
                if (tableMetadata.Columns.Any(f => f.ColumnName == currentProperty.ColumnName) == false)
                    throw new NoMatchingColumnForDBPropertyException(this.TableName, currentProperty.ColumnName);
            }
        }

        #endregion

        #region Loading

        public void Load()
        {
            var tableNameAttribute = typeof(T).GetCustomAttribute<DBEntityTableName>();

            this.TableName = tableNameAttribute != null
                ? tableNameAttribute.TableName
                : typeof(T).Name;

            this.Properties = new List<DBPropertyMapping>();

            this.LoadProperties(typeof(T));
        }

        private void LoadProperties(Type type)
        {
            foreach (var currentProperty in type.GetProperties())
            {
                if (currentProperty.GetCustomAttribute<DBIgnore>() != null)
                    continue;

                var columnNameAttribute = currentProperty.GetCustomAttribute<DBPropertyColumnName>();

                var columnName = columnNameAttribute != null
                    ? columnNameAttribute.TableName
                    : currentProperty.Name;

                var propertyMapping = new DBPropertyMapping
                {
                    ColumnName = columnName,
                    Property = currentProperty
                };

                if (currentProperty.GetCustomAttribute<DBPrimaryKey>() != null)
                {
                    this.PrimaryKeyPropertyMapping = propertyMapping;
                }

                if (currentProperty.GetCustomAttribute<DBProperty>() != null)
                {
                    this.Properties.Add(propertyMapping);
                }
            }

            if (type.BaseType != null)
                this.LoadProperties(type.BaseType);
        }

        #endregion

        #region PrimaryKey

        public object GetPrimaryKeyValue(T entity)
        {
            return this.PrimaryKeyPropertyMapping.Property.GetValue(entity);
        }

        public bool IsPrimaryKeyDefaultValue(T entity)
        {
            return this.GetPrimaryKeyValue(entity).Equals(Activator.CreateInstance(this.PrimaryKeyPropertyMapping.Property.PropertyType));
        }

        #endregion

        public class DBPropertyMapping
        {
            public PropertyInfo Property { get; set; }

            public string ColumnName { get; set; }
        }
    }
}