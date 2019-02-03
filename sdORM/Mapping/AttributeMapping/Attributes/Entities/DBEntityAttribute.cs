using System;

namespace sdORM.Mapping.AttributeMapping.Attributes.Entities
{
    public class DBEntityAttribute : Attribute
    {
        public string TableName { get; }

        public DBEntityAttribute()
        {
            
        }

        public DBEntityAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}