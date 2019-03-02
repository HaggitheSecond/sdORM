using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using sdORM.Session;
using sdORM.Session.Interfaces;

namespace sdORM.Mapping
{
    public abstract class EntityMapping
    {
        protected readonly IList<DBPropertyMapping> _properties;

        public string TableName { get; protected set; }
        public DBPropertyMapping PrimaryKeyPropertyMapping { get; protected set; }
        public ReadOnlyCollection<DBPropertyMapping> Properties { get; }

        public bool HasBeenValidatedAndMapped { get; protected set; }
        public bool HasBeenValidatedAgainstDatabase { get; protected set; }

        protected EntityMapping()
        {
            this.Properties = new ReadOnlyCollection<DBPropertyMapping>(this._properties = new List<DBPropertyMapping>());
        }

        public abstract void ValidateAndMap();

        public abstract void ValidateAgainstDatabase(IDatabaseSession session, ITypeToColumnTypeConverter converter);

        public abstract Task ValidateAgainstDatabase(IDatabaseSessionAsync session, ITypeToColumnTypeConverter converter);
        
        public class DBPropertyMapping
        {
            public PropertyInfo Property { get; set; }

            public string ColumnName { get; set; }
        }
    }

    public abstract class EntityMapping<T> : EntityMapping
    {
        public object GetPrimaryKeyValue(T entity)
        {
            return this.PrimaryKeyPropertyMapping.Property.GetValue(entity);
        }

        public bool IsPrimaryKeyDefaultValue(T entity)
        {
            return this.GetPrimaryKeyValue(entity).Equals(Activator.CreateInstance(this.PrimaryKeyPropertyMapping.Property.PropertyType));
        }
    }
}