using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using sdORM.Attributes;
using sdORM.Interfaces;

namespace sdORM.Entities
{
    public abstract class EntityMapping
    {
        protected readonly IList<DBPropertyMapping> _properties;

        public string TableName { get; protected set; }
        public DBPropertyMapping PrimaryKeyPropertyMapping { get; protected set; }
        public ReadOnlyCollection<DBPropertyMapping> Properties { get; }

        protected EntityMapping()
        {
            this.Properties = new ReadOnlyCollection<DBPropertyMapping>(this._properties = new List<DBPropertyMapping>());
        }

        public abstract void Map();

        public abstract void ValidateAgainstDatabase(IDataBaseSession session);

        public abstract Task ValidateAgainstDatabase(IDataBaseSessionAsync session);

        public class DBPropertyMapping
        {
            public PropertyInfo Property { get; set; }

            public string ColumnName { get; set; }
        }
    }

    public abstract class EntityMapping<T> : EntityMapping
    { 

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
    }
}