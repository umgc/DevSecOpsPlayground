using CsvHelper.Configuration.Attributes;
using StudentReviews.Shared;
using System.ComponentModel.DataAnnotations;

namespace StudentReviews.Data
{
    public class Student
    {
        public Student() 
        {
            AssignedTeam = new Teams();
        }

        [Required]
        [Name("StudentId")] 
        public int StudentId { get; set; }

        [Name("FirstName")]
        public string? FirstName { get; set; }

        [Name("LastName")]
        public string? LastName { get; set; }

        [Name("Email")] 
        public string? Email { get; set; }

        public Teams AssignedTeam { get; set; }
    }
}