using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class TypeToColumnTypeMissmatchException : DBMappingException
    {
        public TypeToColumnTypeMissmatchException(Type entityType, PropertyInfo property, string columnTypeRaw)
         : base($"The type '{entityType.Name}' has a missmatch in property '{property.Name}': type '{property.PropertyType}' does not match column type '{columnTypeRaw}'")
        {
            
        }

        public TypeToColumnTypeMissmatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }
    }
}