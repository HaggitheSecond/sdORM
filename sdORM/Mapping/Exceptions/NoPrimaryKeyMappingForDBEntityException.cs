using System;

namespace sdORM.Mapping.Exceptions
{
    public class NoPrimaryKeyMappingForDBEntityException : Exception
    {
        public NoPrimaryKeyMappingForDBEntityException(Type type)
            : base($"'{type.Name}' does not have a primary key attribute. Try adding [DBPrimaryKey] to your entity.")
        {

        }
    }
}