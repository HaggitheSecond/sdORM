using System;
using System.Reflection;

namespace sdORM.Exceptions
{
    public class NoDBPropertyMappingException : Exception
    {
        public NoDBPropertyMappingException(Type type, MemberInfo property)
            : base($"'{property.Name}' in type '{type.FullName}' is not decleared as a DBProperty or similar. Try adding [DBProperty] or [DBIgnore].")
        {

        }
    }
}