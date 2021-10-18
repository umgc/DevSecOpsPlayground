using System;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    public class FaqInformation
    {
        public Guid Guid { get; set; } = Guid.NewGuid();

        [StringLength(500, MinimumLength = 20, ErrorMessage = "Question must be between 20 and 500 characters long")]
        public string Question { get; set; } = string.Empty;
        [Required]
        public string Author { get; set; } = string.Empty;

        public string Reply { get; set; } = string.Empty;
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Topic must be between 20 and 500 characters long")]
        public string Topic { get; set; } = string.Empty;

    }

}