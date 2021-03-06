﻿using System;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{
    [Serializable]
    public class NoMatchingColumnForDBPropertyException : DBMappingException
    {
        public NoMatchingColumnForDBPropertyException(string tableName, string columnName)
            : base($"No column '{columnName}' in table '{tableName}' found.")
        {
            
        }

        public NoMatchingColumnForDBPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}