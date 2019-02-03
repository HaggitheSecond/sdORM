using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using sdORM.Attributes.Entities;
using sdORM.Attributes.Properties;
using sdORM.Common.SqlSpecifics;
using sdORM.Exceptions;
using sdORM.Extensions;
using sdORM.Interfaces;

namespace sdORM.Entities
{
    public class AttributeEntityMapping<T> : EntityMapping<T>
    {
        public AttributeEntityMapping(IDataBaseSession session) 
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
            if (tableMetadata == null)
                throw new NoTableForDBEntityException(typeof(T), this.TableName);

            var keyPropertyColumnMetaData = tableMetadata.Columns.FirstOrDefault(f => f.ColumnName == this.PrimaryKeyPropertyMapping.ColumnName);

            if (keyPropertyColumnMetaData == null)
                throw new NoMatchingColumnForDBPropertyException(this.TableName, this.PrimaryKeyPropertyMapping.ColumnName);

            foreach (var currentProperty in this.Properties)
            {
                var columnMetaData = tableMetadata.Columns.FirstOrDefault(f => f.ColumnName == currentProperty.ColumnName);

                if (columnMetaData == null)
                    throw new NoMatchingColumnForDBPropertyException(this.TableName, currentProperty.ColumnName);
            }
        }

        #endregion

        #region Loading

        private void Load()
        {
            var tableNameAttribute = typeof(T).GetCustomAttribute<DBEntityTableName>();

            this.TableName = tableNameAttribute != null
                ? tableNameAttribute.TableName
                : typeof(T).Name;
            
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
                    this._properties.Add(propertyMapping);
                    
                }
            }

            if (type.BaseType != null)
                this.LoadProperties(type.BaseType);
        }

        #endregion
    }
}