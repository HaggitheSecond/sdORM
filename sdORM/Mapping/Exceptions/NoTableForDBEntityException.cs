using System;

namespace sdORM.Mapping.Exceptions
{
    public class NoTableForDBEntityException : Exception
    {
        public NoTableForDBEntityException(Type type, string tableName)
            : base($"No matching table for type '{type.Name}' and tableName '{tableName}' found.")
        {
            
        }
    }
}