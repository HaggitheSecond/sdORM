using System;
using sdORM.Common.SqlSpecifics;

namespace sdORM.Mapping
{
    public interface ITypeToColumnTypeConverter
    {
        bool IsValidTypeForColumnType(Type type, ColumnMetaData columnTypeRaw);
    }
}