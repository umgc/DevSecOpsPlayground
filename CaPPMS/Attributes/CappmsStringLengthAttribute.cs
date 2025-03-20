using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Attributes
{
    /// <summary>
    /// Provides a custom string length attribute that includes the current length of the string in the error message.
    /// </summary>
    public class CappmsStringLengthAttribute : StringLengthAttribute
    {
        private string? context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CappmsStringLengthAttribute"/> class.
        /// </summary>
        /// <param name="maximumLength">The maximum length of the object shall be.</param>
        public CappmsStringLengthAttribute(int maximumLength) : base(maximumLength)
        {
        }

        /// <summary>
        /// Override the default IsValid method to store the current string value.
        /// </summary>
        /// <param name="value">The object to validate.</param>
        /// <returns><c>true</c> if valid.</returns>
        public override bool IsValid(object? value)
        {
            if (value is string str)
            {
                this.context = str;
            }

            return base.IsValid(value);
        }

        /// <summary>
        /// Override the default error message to include the current length of the string.
        /// </summary>
        /// <param name="name">Property Name.</param>
        /// <returns>A formatted message.</returns>
        public override string FormatErrorMessage(string name)
        {
            if (this.context != null)
            {
                string message = base.FormatErrorMessage(name);
                message += $" Current length:{this.context.Length}.";
                return message;
            }

            return base.FormatErrorMessage(name);
        }
    }
}
