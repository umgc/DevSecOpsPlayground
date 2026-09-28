using CaPPMS.Model;
using System.ComponentModel.DataAnnotations;


namespace CaPPMS.Attributes
{
    public class AttachmentsNumFilesValidator : ValidationAttribute
    {
        private int maxNumberOfFiles;

        public AttachmentsNumFilesValidator(int maxNumberOfFiles)
        {
            this.maxNumberOfFiles = maxNumberOfFiles;
        }

        public string GetErrorMessage() => $"Exceeded max number of files. Max:{maxNumberOfFiles}.";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (!(validationContext.ObjectInstance is ProjectInformation idea))
            {
                return new ValidationResult("Input is not an idea");
            }

            return idea.Attachments.Count > maxNumberOfFiles ? new ValidationResult(GetErrorMessage()) : ValidationResult.Success;
        }
    }
}
