using System;

namespace sdORM.Mapping.AttributeMapping.Attributes.Entities
{
    /// <summary>
    /// This attribute declares the tablename for an entity. If this attribute is not applied the class-name will be used as the tablename.
    /// </summary>
    public class DBEntityTableName : Attribute
    {
        public string TableName { get; }

        public DBEntityTableName(string tableName)
        {
            this.TableName = tableName;
        }
    }
}