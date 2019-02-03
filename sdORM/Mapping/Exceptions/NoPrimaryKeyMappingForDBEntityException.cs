using System;

namespace sdORM.Mapping.Exceptions
{
    public class NoPrimaryKeyMappingForDBEntityException : Exception
    {
        public NoPrimaryKeyMappingForDBEntityException(Type type)
            : base($"'{type.Name}' does not have a primary key attribute. Try adding [DBPrimaryKeyAttribute] to your entity.")
        {

        }
    }
}