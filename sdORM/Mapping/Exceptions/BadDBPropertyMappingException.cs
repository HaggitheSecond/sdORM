using System;
using System.Reflection;

namespace sdORM.Mapping.Exceptions
{
    public class BadDBPropertyMappingException : Exception
    {
        public BadDBPropertyMappingException(Type type, MemberInfo property, string message)
            : base($"'{property.Name}' in type '{type.FullName}' has bad mapping: {message}")
        {

        }
    }
}