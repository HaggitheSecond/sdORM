using System;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class TypeNotMappedException : Exception
    {
        public TypeNotMappedException(Type type)
            : base($"The type '{type.Name}' has not been mapped in the EntityMappingProvider.")
        {
            
        }
    }
}