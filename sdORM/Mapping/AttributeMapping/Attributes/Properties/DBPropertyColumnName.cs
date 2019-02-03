using System;

namespace sdORM.Mapping.AttributeMapping.Attributes.Properties
{
    /// <summary>
    /// This attribute declares the columnname for a property. If this attribute is not applied the propertyname will be used as the columnname.
    /// </summary>
    public class DBPropertyColumnName : Attribute
    {
        public string TableName { get; }

        public DBPropertyColumnName(string tableName)
        {
            this.TableName = tableName;
        }
    }
}