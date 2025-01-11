using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CaPPMS.Attributes
{
    /// <summary>
    /// Represents an attribute that specifies the range of values that are allowed for a property or parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedStringNumericBasedValuesAttribute : ValidationAttribute
    {
        private readonly int[] range;

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowedStringNumericBasedValuesAttribute" /> class.
        /// </summary>
        /// <param name="start">The bottom of the range.</param>
        /// <param name="end">The top of the range.</param>
        public AllowedStringNumericBasedValuesAttribute(int start, int end)
        {
            this.range = Enumerable.Range(start, end).ToArray();
        }

        /// <summary>
        /// Gets the range of values allowed by this attribute.
        /// </summary>
        public IEnumerable<int> Range => range;

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

            if (int.TryParse(value.ToString(), out int result))
            {
                return range.Any(i => i == result);
            }

            return base.IsValid(value);
        }
    }
}
