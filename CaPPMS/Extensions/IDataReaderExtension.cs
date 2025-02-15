using CaPPMS.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace CaPPMS.Extensions
{
    public static class IDataReaderExtension
    {
        /// <summary>
        /// Converts the row of the <see cref="IDataReader"/> into the specified type.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="dataReader"><see cref="IDataReader"/>.</param>
        /// <returns><typeparamref name="T"/>.</returns>
        public static T ConvertRecord<T>(this IDataReader dataReader, Dictionary<string, int>? columnMap = null) where T : new()
        {
            ArgumentNullException.ThrowIfNull(dataReader);

            if (columnMap == null)
            {
                columnMap = dataReader.GetColumnMap();
            }

            T record = new();
            typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop =>
                {
                    return prop.GetCustomAttribute<IgnoreDataMemberAttribute>() == null;
                })
                .Foreach(property =>
                {
                    if (!columnMap.TryGetValue(property.Name, out int columnId))
                    {
                        return;
                    }

                    object? value = dataReader.GetValue(columnId);
                    if (value != DBNull.Value)
                    {
                        if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(record, Convert.ToDouble(value));
                        }
                        else if (property.PropertyType == typeof(List<string>))
                        {
                            property.SetValue(record, value?.ToString()?.Split(',').ToList());
                        }
                        else
                        {
                            property.SetValue(record, ConvertValue(value, property.PropertyType));
                        }
                    }
                });

            return record;
        }

        /// <summary>
        /// Get the column map.
        /// </summary>
        /// <param name="dataReader">This <see cref="IDataReader"/>.</param>
        /// <returns>Column Map.</returns>
        public static Dictionary<string, int> GetColumnMap(this IDataReader dataReader)
        {
            ArgumentNullException.ThrowIfNull(dataReader);
            Dictionary<string, int> columnMap = new(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                columnMap.Add(dataReader.GetName(i), i);
            }

            return columnMap;
        }

        private static object ConvertValue(object value, Type expectedType)
        {
            // Trying to auto convert to DateTime? fails.
            if (value.TryConvertDateTime(out DateTime? result))
            {
                if (result == null)
                {
                    return DateTime.MinValue;
                }

                return result;
            }

            return value;
        }
    }
}
