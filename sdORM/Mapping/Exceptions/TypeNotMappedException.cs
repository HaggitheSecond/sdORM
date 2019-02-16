using System;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class TypeNotMappedException : DBMappingException
    {
        public TypeNotMappedException(Type type)
            : base($"The type '{type.Name}' has not been mapped in the EntityMappingProvider.")
        {
            
        }

        public TypeNotMappedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}