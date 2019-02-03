using System;
using System.Reflection;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class DBEntityNonValidTypeException : DBMappingException
    {
        public DBEntityNonValidTypeException(MemberInfo type)
            : base($"Type '{type.Name}' is not a valid type for a DBEntity.")
        {

        }
    }
}