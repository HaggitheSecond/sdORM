using System;

namespace sdORM.Mapping.AttributeMapping.Attributes.Properties
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DBPrimaryKeyAttribute : Attribute
    {
        public string ColumnName { get; }

        public DBPrimaryKeyAttribute()
        {

        }

        public DBPrimaryKeyAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}