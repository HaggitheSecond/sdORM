using System;

namespace sdORM.Exceptions
{
    public class NoPrimaryKeyMappingForDBEntityException : Exception
    {
        public NoPrimaryKeyMappingForDBEntityException(Type type)
            : base($"'{type.Name}' does not have a primary key attribute. Try adding [DBPrimaryKey] to your entity.")
        {

        }
    }
}