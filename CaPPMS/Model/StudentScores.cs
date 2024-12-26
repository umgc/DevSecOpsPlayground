using System.Collections.Generic;

namespace CaPPMS.Model
{
    public class StudentScores : Student
    {
        public StudentScores() { }

        /// <summary>
        /// Current Score.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Average Score.
        /// </summary>
        public double AverageScore { get; set; }

        /// <summary>
        /// Comment.
        /// </summary>
        public List<string> Comment { get; set; } = [];

        /// <summary>
        /// Week of class.
        /// </summary>
        public string Week { get; set; } = string.Empty;
    }
}