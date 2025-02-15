using CaPPMS.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CaPPMS.Model
{
    [SqlTableName("StudentReviews")]
    public class Review : ISqlTableModel
    {
        public Review() { }

        [SqlIdProperty]
        public long StudentReviewId { get; set; } = -1;

        [Required(ErrorMessage = "Student selection is required")]
        public long ReviewedStudentId { get; set; } = -1;

        public long ReviewedById { get; set; } = -1;

        [IgnoreDataMember]
        public string RatedStudent { get; set; } = string.Empty;

        [Required(ErrorMessage = "Week selection is required")]
        [AllowedStringNumericBasedValues(1, 12, ErrorMessage = "Please select a week between 1 and 10.")]
        public string Week { get; set; } = "1";

        [Required(ErrorMessage = "Score is required")]
        [AllowedStringNumericBasedValues(0, 100, ErrorMessage = "Please select a score between 0 and 100.")]
        public string Score { get; set; } = "0";

        [Required(ErrorMessage = "Your comments for the student are appreciated.", AllowEmptyStrings = false)]
        [StringLength(maximumLength: 255, MinimumLength = 25, ErrorMessage = "Please use at least 25 characters to describe the interaction of this student.")]
        public string Comments { get; set; } = string.Empty;
    }
}