using MySql.Data.Types;
using System;
using System.Globalization;
using MySql.Data.MySqlClient;
using sdORM.Common.SqlSpecifics;
using sdORM.Mapping;

namespace sdORM.MySql
{
    public class TypeToMySqlColumnTypeConverter : ITypeToColumnTypeConverter
    {
        private MySqlDbType ParseDbType(string columnTypeRaw)
        {
            //TODO: This does not work at the moment - needs an actual way to find out if it isUnsigned
            var isUnsigned = false;
            switch (columnTypeRaw.ToUpper(CultureInfo.InvariantCulture))
            {
                case "BIT":
                    return MySqlDbType.Bit;
                case "BOOL":
                case "BOOLEAN":
                    return MySqlDbType.Byte;

                case "NUMERIC":
                case "DECIMAL":
                case "DEC":
                case "FIXED":
                    return MySqlDbType.Decimal;

                case "FLOAT":
                    return MySqlDbType.Float;
                case "DOUBLE":
                case "REAL":
                    return MySqlDbType.Double;

                case "YEAR":
                    return MySqlDbType.Year;
                case "TIME":
                    return MySqlDbType.Time;
                case "TIMESTAMP":
                    return MySqlDbType.Timestamp;
                case "DATE":
                    return MySqlDbType.Date;
                case "DATETIME":
                    return MySqlDbType.DateTime;
                    
                case "TINYINT":
                    return isUnsigned ? MySqlDbType.UByte : MySqlDbType.Byte;
                case "SMALLINT":
                    return isUnsigned ? MySqlDbType.UInt16 : MySqlDbType.Int16;
                case "MEDIUMINT":
                    return isUnsigned ? MySqlDbType.UInt24 : MySqlDbType.Int24;
                case "INT":
                case "INTEGER":
                    return isUnsigned ? MySqlDbType.UInt32 : MySqlDbType.Int32;
                case "SERIAL":
                    return MySqlDbType.UInt64;
                case "BIGINT":
                    return isUnsigned ? MySqlDbType.UInt64 : MySqlDbType.Int64;
                    
                case "TINYTEXT":
                    return MySqlDbType.TinyText;
                case "MEDIUMTEXT":
                    return MySqlDbType.MediumText;
                case "LONGTEXT":
                    return MySqlDbType.LongText;
                case "TEXT":
                    return MySqlDbType.Text;
                case "SET":
                    return MySqlDbType.Set;
                case "ENUM":
                    return MySqlDbType.Enum;
                case "CHAR":
                    return MySqlDbType.String;
                case "VARCHAR":
                    return MySqlDbType.VarChar;

                case "BLOB":
                    return MySqlDbType.Blob;
                case "LONGBLOB":
                    return MySqlDbType.LongBlob;
                case "MEDIUMBLOB":
                    return MySqlDbType.MediumBlob;
                case "TINYBLOB":
                    return MySqlDbType.TinyBlob;

                case "BINARY":
                    return MySqlDbType.Binary;
                case "VARBINARY":
                    return MySqlDbType.VarBinary;

                default:
                    throw new ArgumentException($"Columntype {columnTypeRaw} is not supported ");
            }
        }

        public bool IsValidTypeForColumnType(Type propertyType, ColumnMetaData columnTypeRaw)
        {
            var columnType = this.ParseDbType(columnTypeRaw.DataType);

            switch (columnType)
            {
                case MySqlDbType.Bit:
                    return propertyType == typeof(bool);

                case MySqlDbType.Byte:
                    return propertyType == typeof(sbyte);
                case MySqlDbType.UByte:
                    return propertyType == typeof(byte);

                case MySqlDbType.UInt16:
                    return propertyType == typeof(ushort);
                case MySqlDbType.Year:
                case MySqlDbType.Int16:
                    return propertyType == typeof(short);

                case MySqlDbType.Int24: // not sure about this
                case MySqlDbType.Int32:
                    return propertyType == typeof(int);

                case MySqlDbType.UInt24:
                case MySqlDbType.UInt32:
                    return propertyType == typeof(uint);

                case MySqlDbType.UInt64:
                    return propertyType == typeof(ulong);
                case MySqlDbType.Int64:
                    return propertyType == typeof(long);

                case MySqlDbType.NewDecimal:
                case MySqlDbType.Decimal:
                    return propertyType == typeof(decimal);

                case MySqlDbType.Float:
                    return propertyType == typeof(float);

                case MySqlDbType.Double:
                    return propertyType == typeof(double);

                case MySqlDbType.Newdate:
                case MySqlDbType.Date:
                case MySqlDbType.DateTime:
                case MySqlDbType.Timestamp:
                    return propertyType == typeof(DateTime);

                case MySqlDbType.Time:
                    return propertyType == typeof(TimeSpan);

                case MySqlDbType.String:
                case MySqlDbType.Text:
                case MySqlDbType.TinyText:
                case MySqlDbType.MediumText:
                case MySqlDbType.LongText:
                case MySqlDbType.VarChar:
                case MySqlDbType.VarString:
                case MySqlDbType.Set:
                case MySqlDbType.Enum:
                    return propertyType == typeof(string) || propertyType.IsEnum;

                case MySqlDbType.VarBinary:
                case MySqlDbType.Binary:
                case MySqlDbType.TinyBlob:
                case MySqlDbType.MediumBlob:
                case MySqlDbType.LongBlob:
                case MySqlDbType.Blob:
                    return propertyType == typeof(byte[]);

                case MySqlDbType.Guid:
                    return propertyType == typeof(Guid);

                case MySqlDbType.Geometry:
                    return propertyType == typeof(object);

                case MySqlDbType.JSON:
                    return propertyType == typeof(object);

                default:
                    throw new ArgumentOutOfRangeException(nameof(columnType), columnType, null);
            }
        }
    }
}