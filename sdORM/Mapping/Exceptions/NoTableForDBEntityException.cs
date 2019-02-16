using System;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class NoTableForDBEntityException : DBMappingException
    {
        public NoTableForDBEntityException(Type type, string tableName)
            : base($"No matching table for type '{type.Name}' and tableName '{tableName}' found.")
        {
            
        }

        public NoTableForDBEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}