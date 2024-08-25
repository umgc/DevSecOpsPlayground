namespace CaPPMS.Data
{
    public class StudentScores
    {
        public StudentScores(long id, string firstName, string lastName)
        {
            this.StudentId = id;
        }

        /// <summary>
        /// Student ID
        /// </summary>
        public long StudentId { get; private set; }

        /// <summary>
        /// Student First name.
        /// </summary>
        public string FirstName { get; private set; } = string.Empty;

        /// <summary>
        /// Student last name.
        /// </summary>
        public string LastName { get; private set; } = string.Empty;

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
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Week of class.
        /// </summary>
        public string Week { get; set; } = string.Empty;
    }
}