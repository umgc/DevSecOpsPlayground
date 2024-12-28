using CaPPMS.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    [SqlTableName("StudentReviews")]
    public class Review
    {
        public Review() { }

        [SqlIdProperty]
        public int StudentReviewId { get; set; } = -1;

        [Required(ErrorMessage = "Student selection is required")]
        public int ReviewedStudentId { get; set; } = -1;

        [Required(ErrorMessage = "Week selection is required")]
        [Range(0, 12, ErrorMessage = "The value must be between 0 and 12.")]
        public int Week { get; set; }

        [Required(ErrorMessage = "Score is required")]
        [Range(0, 100, ErrorMessage = "The value must be between 0 and 100.")]
        public int Score { get; set; }

        [Required(ErrorMessage = "Your comments for the student are appreciated.", AllowEmptyStrings = false)]
        [StringLength(maximumLength: 255, MinimumLength = 25, ErrorMessage = "Please use at least 25 characters to describe the interaction of this student.")]
        public string Comments { get; set; } = string.Empty;
    }
}