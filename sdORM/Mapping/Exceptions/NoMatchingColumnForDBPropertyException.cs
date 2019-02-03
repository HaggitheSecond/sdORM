using System;

namespace sdORM.Mapping.Exceptions
{
    public class NoMatchingColumnForDBPropertyException : Exception
    {
        public NoMatchingColumnForDBPropertyException(string tableName, string columnName)
            : base($"No column '{columnName}' in table '{tableName}' found.")
        {
            
        }
    }
}