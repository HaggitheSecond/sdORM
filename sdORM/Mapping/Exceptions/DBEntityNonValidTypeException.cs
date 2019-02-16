using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class DBEntityNonValidTypeException : DBMappingException
    {
        public DBEntityNonValidTypeException(MemberInfo type)
            : base($"Type '{type.Name}' is not a valid type for a DBEntity.")
        {

        }

        public DBEntityNonValidTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}