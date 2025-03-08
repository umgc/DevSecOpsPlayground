using CaPPMS.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaPPMS.Pages.StudentReviews
{
    public partial class StudentReview
    {
        private string username = string.Empty;
        private string log = string.Empty;
        private int count = -1;
        private bool isSubmitButtonDisabled = true;

        public Student student = new Student();

        public List<Student>? ClassList { get; set; }

        public List<string>? WeeksList { get; set; }

        public string? StatusMessage { get; set; }

        /// <summary>
        /// Submit the evaluation.
        /// </summary>
        /// <returns><c>true</c> if successful.</returns>
        public async Task<bool> SubmitEvaluation()
        {
            string? studentName = await LookupStudentAsync(review.ReviewedStudentId);
            if (string.IsNullOrEmpty(studentName))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(reviewWeek))
            {
                review.Week = reviewWeek;
            }

            if (await DBOperationsService.AddRecord(review))
            {
                review.RatedStudent = studentName;
                return true;
            }

            return false;
        }

        public async Task<string?> LookupStudentAsync(long studentId)
        {
            Student? student = (await DBOperationsService.GetRecords<Student>(studentId)).FirstOrDefault();

            if (student == null)
            {
                return null;
            }

            return student.FirstName + " " + student.LastName;
        }
    }
}