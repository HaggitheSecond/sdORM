using System;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class DBMappingException : Exception
    {
        public DBMappingException()
        {
        }

        public DBMappingException(string message)
            : base(message)
        {
        }

        public DBMappingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DBMappingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}