using System;

namespace sdORM.Mapping.AttributeMapping.Attributes.Properties
{
    public class DBPropertyAttribute : Attribute
    {
        public string ColumnName { get; }

        public DBPropertyAttribute()
        {

        }

        public DBPropertyAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}