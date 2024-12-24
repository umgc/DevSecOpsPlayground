using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CaPPMS.Extensions
{
    public static class UtilityExtensions
    {
        /// <summary>
        /// Check if the object matches the filter.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool IsMatch(this object item, string filter)
        {
            if (item == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(filter))
            {
                return true;
            }

            PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            string[] filterParts = filter.Split(new char[] {' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            bool propertyMatch = filterParts.All(part =>
            {
                return properties.Any(property =>
                {
                    object? value = property.GetValue(item);
                    if (value == null)
                    {
                        return true;
                    }

                    if (value is IEnumerable enemerable)
                    {
                        return string.Join(" ", enemerable).IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                    return value.NullSafeToString().IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0;
                });
            });

            return propertyMatch;
        }

        public static IEnumerable<T> Foreach<T>(this IEnumerable<T> value, Action<T> action)
        {
            if (value == null)
            {
                return [];
            }

            foreach (T item in value)
            {
                action?.Invoke(item);
            }

            return value;
        }

        /// <summary>
        /// Return null safe object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string NullSafeToString(this object? obj)
        {
            return obj?.ToString() ?? "(null)";
        }

        public static bool TryConvertDateTime(this object? date, out DateTime? result)
        {
            result = null;
            if (date == null)
            {
                return false;
            }

            if (DateTime.TryParse(date.NullSafeToString(), out DateTime parsedDate))
            {
                result = parsedDate;
                return true;
            }

            return false;
        }
    }
}
