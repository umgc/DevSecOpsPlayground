using System;
using System.Data;

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

        /// <summary>
        /// Get <see cref="StudentScores"/> from the <see cref="IDataReader"/>.
        /// </summary>
        /// <param name="dataReader">Active <see cref="IDataReader"/>.</param>
        /// <returns><see cref="StudentScores"/>.</returns>
        public static StudentScores GetStudentScores(IDataReader dataReader)
        {
            if (dataReader.IsClosed)
            {
                throw new InvalidOperationException("Data Reader is closed.");
            }

            return new StudentScores(
                Convert.ToInt64(dataReader[nameof(StudentId)]),
                dataReader[nameof(FirstName)].NullSafeToString(),
                dataReader[nameof(LastName)].NullSafeToString())
            {
                AverageScore = dataReader[nameof(AverageScore)] == DBNull.Value
                ? 0
                : Convert.ToInt32(dataReader[nameof(AverageScore)]),
                Week = dataReader[nameof(Week)].NullSafeToString(),
                Score = dataReader[nameof(Score)] == DBNull.Value
                ? 0
                : Convert.ToDouble(dataReader[nameof(Score)]),
                Comment = dataReader[nameof(Comment)].NullSafeToString()
            };
        }
    }
}