using CaPPMS.Extensions;
using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Attributes
{
    /// <summary>
    /// Represents an attribute that specifies the range of values that are allowed for a property or parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedStringNumericBasedValuesAttribute : ValidationAttribute
    {
        private HashSet<string> stringLookup = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HashSet<int> intLookup = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowedStringNumericBasedValuesAttribute" /> class.
        /// </summary>
        /// <param name="start">The bottom of the range.</param>
        /// <param name="end">The top of the range.</param>
        public AllowedStringNumericBasedValuesAttribute(int start, int end)
        {
            while(start <= end)
            {
                stringLookup.Add(start.ToWords());
                intLookup.Add(start);
                start++;
            }
        }

        /// <summary>
        /// Determines whether a specified object is valid. (Overrides <see cref="ValidationAttribute.IsValid(object)" />)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return false;
            }

            if (stringLookup.Contains(value.NullSafeToString()))
            {
                return true;
            }

            if (int.TryParse(value.NullSafeToString(), out int result) && intLookup.Contains(result))
            {
                return true;
            }

            return false;
        }
    }
}
