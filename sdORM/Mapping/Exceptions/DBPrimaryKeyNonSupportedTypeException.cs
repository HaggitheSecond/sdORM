using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class DBPrimaryKeyNonSupportedTypeException : DBMappingException
    {
        public DBPrimaryKeyNonSupportedTypeException(MemberInfo type, PropertyInfo property)
            : base($"'{property.PropertyType}' for type {type.Name} is not supported as a primary key.")
        {

        }

        public DBPrimaryKeyNonSupportedTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}