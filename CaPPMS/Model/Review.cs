using CaPPMS.Data;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    public class Review
    {
        private int classId;
        DBOperationsService dbOperationsService;

        public Review() { }

        public Review(int classId, DBOperationsService dB)
        {
            this.classId = classId;
            dbOperationsService = dB;
        }

        public int StudentReviewId { get; set; } = -1;

        [Required(ErrorMessage = "Student selection is required")]
        public int ReviewedStudentId { get; set; } = -1;

        [Required(ErrorMessage = "Week selection is required")]
        public string Week { get; set; } = string.Empty;

        [Required(ErrorMessage = "Score is required")]
        [Range(0, 100, ErrorMessage = "The value must be between 0 and 100.")]
        public string Score { get; set; } = string.Empty;

        public string Comments { get; set; } = string.Empty;
    }
}