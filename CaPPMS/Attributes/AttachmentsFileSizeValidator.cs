using CaPPMS.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace CaPPMS.Attributes
{
    public class AttachmentsFileSizeValidator : ValidationAttribute
    {
        private int maxFileSize;

        private List<string> badFiles = new List<string>();

        public AttachmentsFileSizeValidator(int maxFileSizeMb)
        {
            this.maxFileSize = maxFileSizeMb;
        }

        public string GetErrorMessage()
        {
            string files = string.Join(",", badFiles);

            return $"Max file size ({maxFileSize}) exceeded on: {files}.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var idea = validationContext.ObjectInstance as ProjectInformation;

            foreach(var file in idea.Attachments)
            {
                if(file.Size > this.maxFileSize * 1024 * 1024)
                {
                    badFiles.Add(file.Name);
                }
            }

            if (badFiles.Count > 0)
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }
    }
}
