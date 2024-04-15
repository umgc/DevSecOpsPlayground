using System.ComponentModel.DataAnnotations;

namespace StudentReviews.Data
{
    public class Review
    {
        public int StudentReviewId { get; set; }

        [Required(ErrorMessage = "Student selection is required")]
        public string ReviewedStudentId { get; set; }

        public string ReviewersEmail { get; set; }

        [Required(ErrorMessage = "Week selection is required")]
        public string Week { get; set; }

        [Required(ErrorMessage = "Score is required")]
        [Range(0, 100, ErrorMessage = "The value must be between 0 and 100.")]
        public string Score { get; set; }

        public string Comments { get; set; }
    }
}