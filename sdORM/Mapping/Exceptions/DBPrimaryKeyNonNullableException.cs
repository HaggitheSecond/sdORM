using System;
using System.Reflection;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class DBPrimaryKeyNonNullableException : DBMappingException
    {
        public DBPrimaryKeyNonNullableException(MemberInfo type, MemberInfo property)
            : base($"The primary key for {type.Name} and property {property.Name} must be a non-nullable type.")
        {

        }
    }
}