using System;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class NoDBEntityMappingException : DBMappingException
    {
        public NoDBEntityMappingException(Type type)
            : base($"'{type.FullName}' is not decleared as a DBEntityAttribute. Try adding the [DBEntityAttribute] attribute.")
        {

        }

        public NoDBEntityMappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}