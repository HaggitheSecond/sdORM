using System;
using System.Reflection;

namespace sdORM.Mapping.Exceptions
{
    public class NoDBPropertyMappingException : Exception
    {
        public NoDBPropertyMappingException(Type type, MemberInfo property)
            : base($"'{property.Name}' in type '{type.FullName}' is not decleared as a DBPropertyAttribute or similar. Try adding [DBPropertyAttribute] or [DBIgnoreAttribute].")
        {

        }
    }
}