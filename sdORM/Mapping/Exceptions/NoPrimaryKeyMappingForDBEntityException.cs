using System;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class NoPrimaryKeyMappingForDBEntityException : DBMappingException
    {
        public NoPrimaryKeyMappingForDBEntityException(Type type)
            : base($"'{type.Name}' does not have a primary key attribute. Try adding [DBPrimaryKeyAttribute] to your entity.")
        {

        }
    }
}