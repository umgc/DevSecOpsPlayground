using CaPPMS.Attributes;
using CaPPMS.Model;
using System;
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
        private string SelectedWeek = string.Empty;

        public Student student = new Student();

        public Review review = new Review();

        public List<Review> CompletedReviews = [];

        public List<Student>? ClassList { get; set; }

        public List<string>? WeeksList { get; set; }

        public string? StatusMessage { get; set; }

        public bool HidePanel { get; set; } = true;

        /// <summary>
        /// Submit the evaluation.
        /// </summary>
        /// <returns><c>true</c> if successful.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> SubmitEvaluation()
        {
            string? studentName = await LookupStudentAsync(review.ReviewedStudentId);
            if (string.IsNullOrEmpty(studentName))
            {
                return false;
            }

            if (await DBOperationsService.AddRecord(review))
            {
                review.RatedStudent = studentName;
                HidePanel = false;
                return true;
            }

            return false;
        }

        public async Task<string?> LookupStudentAsync(int studentId)
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