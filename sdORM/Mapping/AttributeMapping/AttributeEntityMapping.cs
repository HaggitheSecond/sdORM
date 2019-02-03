using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using sdORM.Common.SqlSpecifics;
using sdORM.Extensions;
using sdORM.Mapping.AttributeMapping.Attributes.Entities;
using sdORM.Mapping.AttributeMapping.Attributes.Properties;
using sdORM.Mapping.Exceptions;
using sdORM.Session;

namespace sdORM.Mapping.AttributeMapping
{
    public class AttributeEntityMapping<T> : EntityMapping<T>
    {
        public override void Map()
        {
            this.ValidateEntity();
            this.Load();
            this.ValidateAfterLoad();
        }

        #region Validation

        private void ValidateEntity()
        {
            var type = typeof(T);

            if (type.GetCustomAttribute<DBEntityAttribute>() == null)
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
            if (property.GetCustomAttribute<DBPropertyAttribute>() == null
                && property.GetCustomAttribute<DBIgnoreAttribute>() == null
                && property.GetCustomAttribute<DBPrimaryKeyAttribute>() == null)
                throw new NoDBPropertyMappingException(property.DeclaringType, property);

            if (property.GetCustomAttribute<DBPrimaryKeyAttribute>() != null)
            {
                if (property.PropertyType.IsValueType == false || property.PropertyType.IsNullable())
                    throw new DBPrimaryKeyNonNullableException(property.DeclaringType, property);

                if (property.PropertyType != typeof(int))
                    throw new DBPrimaryKeyNonSupportedTypeException(property.DeclaringType, property);
            }
        }

        private void ValidateAfterLoad()
        {
            if (this.PrimaryKeyPropertyMapping == null)
                throw new NoPrimaryKeyMappingForDBEntityException(typeof(T));
        }

        public override void ValidateAgainstDatabase(IDataBaseSession session)
        {
            this.ValidateAgainstTableMetaData(session.GetTableMetaData(this.TableName));
        }

        public override async Task ValidateAgainstDatabase(IDataBaseSessionAsync session)
        {
            this.ValidateAgainstTableMetaData(await session.GetTableMetaDataAsync(this.TableName));
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
            var entityAttribute = typeof(T).GetCustomAttribute<DBEntityAttribute>();
            this.TableName = string.IsNullOrWhiteSpace(entityAttribute.TableName)
                ? entityAttribute.TableName
                : typeof(T).Name;

            this.LoadProperties(typeof(T));
        }

        private void LoadProperties(Type type)
        {
            foreach (var currentProperty in type.GetProperties())
            {
                if (currentProperty.GetCustomAttribute<DBIgnoreAttribute>() != null)
                    continue;


                var primaryKeyAttribute = currentProperty.GetCustomAttribute<DBPrimaryKeyAttribute>();
                if (primaryKeyAttribute != null)
                {
                    var columnName = string.IsNullOrWhiteSpace(primaryKeyAttribute.ColumnName)
                        ? primaryKeyAttribute.ColumnName
                        : currentProperty.Name;

                    this._properties.Add(new DBPropertyMapping
                    {
                        ColumnName = columnName,
                        Property = currentProperty
                    });
                }

                var propertyAttribute = currentProperty.GetCustomAttribute<DBPropertyAttribute>();
                if (propertyAttribute != null)
                {
                    var columnName = string.IsNullOrWhiteSpace(propertyAttribute.ColumnName)
                        ? propertyAttribute.ColumnName
                        : currentProperty.Name;

                    this._properties.Add(new DBPropertyMapping
                    {
                        ColumnName = columnName,
                        Property = currentProperty
                    });
                }
            }

            if (type.BaseType != null)
                this.LoadProperties(type.BaseType);
        }

        #endregion
    }
}