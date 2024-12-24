using System;
using System.Data;
using System.Reflection;

namespace CaPPMS.Extensions
{
    public static class IDataReaderExtension
    {
        public static T ConvertRecord<T>(this IDataReader dataReader) where T : new()
        {
            ArgumentNullException.ThrowIfNull(dataReader);

            T record = new();
            typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Foreach(property =>
                {
                    object? value = dataReader[property.Name];
                    if (value != DBNull.Value)
                    {
                        property.SetValue(record, ConvertValue(value, property.PropertyType));
                    }
                });

            return record;
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
