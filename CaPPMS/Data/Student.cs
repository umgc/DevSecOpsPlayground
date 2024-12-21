using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CaPPMS.Data
{
    public class Student
    {
        [Required]
        public long StudentId { get; set; } = default(long);

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public Team AssignedTeam { get; set; } = new Team();

        public static Student GetStudent(IDataReader dataReader)
        {
            if (dataReader.IsClosed)
            {
                throw new InvalidOperationException("DB is closed.");
            }

            Student student = new Student()
            {
                StudentId = Convert.ToInt64(dataReader[nameof(StudentId)]),
                FirstName = dataReader[nameof(FirstName)].NullSafeToString(),
                LastName = dataReader[nameof(LastName)].NullSafeToString()
            };

            student.AssignedTeam.TeamId = Convert.ToInt32(dataReader[nameof(Team.TeamId)]);

            return student;
        }
    }
}