﻿using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace sdORM.Mapping.Exceptions
{

    [Serializable]
    public class DBPropertyNonValidTypeException : DBMappingException
    {
        public DBPropertyNonValidTypeException(MemberInfo type, PropertyInfo property)
            : base($"Type '{property.PropertyType.Name}' of '{property.Name}' in DBEntity '{type.Name}' is not a valid type.")
        {

        }

        public DBPropertyNonValidTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}