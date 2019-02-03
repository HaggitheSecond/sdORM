using System;
using System.Reflection;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class NoDBPropertyMappingException : DBMappingException
    {
        public NoDBPropertyMappingException(Type type, MemberInfo property)
            : base($"'{property.Name}' in type '{type.FullName}' is not decleared as a DBPropertyAttribute or similar. Try adding [DBPropertyAttribute] or [DBIgnoreAttribute].")
        {

        }
    }
}