using System;
using System.ComponentModel;
using System.Data;
using sdORM.Common.SqlSpecifics;
using sdORM.Mapping;

namespace sdORM.Extensions
{
    public static class IDataRecordExtensions
    {
        public static T LoadWithEntityMapping<T>(this IDataRecord self, EntityMapping<T> mapping) where T : new()
        {
            var entity = new T();

            var idValue = self.GetValue(self.GetOrdinal(mapping.PrimaryKeyPropertyMapping.ColumnName));
            mapping.PrimaryKeyPropertyMapping.Property.SetValue(entity, idValue);

            foreach (var currentProperty in mapping.Properties)
            {
                var value = self.GetValue(self.GetOrdinal(currentProperty.ColumnName));

                if (value == DBNull.Value)
                    continue;

                if (currentProperty.Property.PropertyType.IsEnum)
                {
                    value = TypeDescriptor.GetConverter(currentProperty.Property.PropertyType).ConvertFrom(value);
                }

                currentProperty.Property.SetValue(entity, value);
            }

            return entity;
        }

        public static ColumnMetaData LoadColumnMetaData(this IDataRecord self)
        {
            return new ColumnMetaData
            {
                ColumnName = self.GetString("column_name"),
                OrdinalPosition = self.GetInt32("ordinal_position"),
                DataType = self.GetString("data_type"),
                ColumnType = self.GetString("column_type"),
            };
        }
        
        public static string GetString(this IDataRecord self, string columnName)
        {
            return self.GetString(self.GetOrdinal(columnName));
        }

        public static int GetInt32(this IDataRecord self, string columnName)
        {
            return self.GetInt32(self.GetOrdinal(columnName));
        }
    }
}