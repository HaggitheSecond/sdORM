using System;
using System.Collections.Generic;
using System.Data;

namespace sdORM.Helper
{
    public static class SqlDbTypeHelper
    {
        public static Dictionary<Type, SqlDbType> _types => new Dictionary<Type, SqlDbType>
        {
            { typeof(string), SqlDbType.NVarChar },
            { typeof(bool), SqlDbType.Bit },

            { typeof(byte), SqlDbType.TinyInt },
            { typeof(short), SqlDbType.SmallInt },
            { typeof(int), SqlDbType.Int },
            { typeof(long), SqlDbType.BigInt },

            { typeof(decimal), SqlDbType.Money },
            { typeof(float), SqlDbType.Real },
            { typeof(double), SqlDbType.Float },

            { typeof(DateTime), SqlDbType.DateTime },
            { typeof(DateTimeOffset), SqlDbType.DateTimeOffset },
            { typeof(TimeSpan), SqlDbType.Time },
        };

        public static bool IsValidSqlDbType(Type type)
        {
            if (type.IsEnum)
                return true;

            return _types.ContainsKey(type);
        }
    }
}