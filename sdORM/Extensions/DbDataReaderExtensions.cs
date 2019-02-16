using System;
using System.Collections.Generic;
using System.Data.Common;

namespace sdORM.Extensions
{
    public static class DbDataReaderExtensions
    {
        public static IEnumerable<(int ordinal, string columnName)> GetColumns(this DbDataReader self)
        {
            for (var i = 0; i < self.FieldCount; i++)
            {
                yield return (i, self.GetName(i));
            }
        }
    }
}