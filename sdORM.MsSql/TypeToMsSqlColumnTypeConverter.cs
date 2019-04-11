using System;
using System.Data;
using System.Globalization;
using sdORM.Common.SqlSpecifics;
using sdORM.Mapping;

namespace sdORM.MsSql
{
    public class TypeToMsSqlColumnTypeConverter : ITypeToColumnTypeConverter
    {
        public bool IsValidTypeForColumnType(Type type, ColumnMetaData columnTypeRaw)
        {
            var columnType = this.ParseDbType(columnTypeRaw.DataType);

            switch (columnType)
            {
                case SqlDbType.Bit:
                    return type == typeof(bool);
                    
                case SqlDbType.BigInt:
                    return type == typeof(long);
                    
                case SqlDbType.SmallInt:
                    return type == typeof(short);

                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                    return type == typeof(DateTime);
                case SqlDbType.DateTimeOffset:
                    return type == typeof(DateTimeOffset);

                case SqlDbType.SmallMoney:
                case SqlDbType.Money:
                case SqlDbType.Real:
                case SqlDbType.Decimal:
                    return type == typeof(decimal);
                case SqlDbType.Float:
                    return type == typeof(float);

                case SqlDbType.Int:
                    return type == typeof(int);

                case SqlDbType.NChar:
                case SqlDbType.Char:
                    return type == typeof(char);

                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                    return type == typeof(string) || type.IsEnum;

                case SqlDbType.Time:
                case SqlDbType.Timestamp:
                    return type == typeof(TimeSpan);

                case SqlDbType.TinyInt:
                    return type == typeof(short);

                case SqlDbType.VarBinary:
                case SqlDbType.Binary:
                case SqlDbType.Image:
                    return type == typeof(byte[]);

                default:
                    throw new ArgumentOutOfRangeException(nameof(columnType), columnType, null);
            }
        }

        private SqlDbType ParseDbType(string columnTypeRaw)
        {
            switch (columnTypeRaw.ToUpper(CultureInfo.InvariantCulture))
            {
                case "BIT":
                    return SqlDbType.Bit;

                case "INT":
                    return SqlDbType.Int;

                case "BIGINT":
                    return SqlDbType.BigInt;

                case "NUMERIC":
                case "DECIMAL":
                    return SqlDbType.Decimal;

                case "FLOAT":
                    return SqlDbType.Float;
                case "REAL":
                    return SqlDbType.Real;

                case "TIME":
                    return SqlDbType.Time;
                case "TIMESTAMP":
                    return SqlDbType.Timestamp;
                case "DATE":
                    return SqlDbType.Date;
                case "DATETIME":
                    return SqlDbType.DateTime;

                case "CHAR":
                    return SqlDbType.Char;
                case "VARCHAR":
                    return SqlDbType.VarChar;
                case "TEXT":
                    return SqlDbType.Text;

                case "NCHAR":
                    return SqlDbType.NChar;
                case "NVARCHAR":
                    return SqlDbType.NVarChar;
                case "NTEXT":
                    return SqlDbType.NText;

                case "BINARY":
                    return SqlDbType.Binary;
                case "VARBINARY":
                    return SqlDbType.VarBinary;

                default:
                    throw new ArgumentException($"Columntype {columnTypeRaw} is not supported ");
            }
        }
    }
}