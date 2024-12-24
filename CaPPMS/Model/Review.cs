using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    public class Review
    {
        public int StudentReviewId { get; set; }

        [Required(ErrorMessage = "Student selection is required")]
        public int ReviewedStudentId { get; set; } = -1;

        public string ReviewersEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Week selection is required")]
        public string Week { get; set; } = string.Empty;

        [Required(ErrorMessage = "Score is required")]
        [Range(0, 100, ErrorMessage = "The value must be between 0 and 100.")]
        public string Score { get; set; } = string.Empty;

        public string Comments { get; set; } = string.Empty;
    }
}