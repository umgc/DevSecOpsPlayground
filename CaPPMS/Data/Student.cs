using CaPPMS.Shared;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Data
{
    public class Student
    {
        public Student() 
        {
            AssignedTeam = new Teams();
        }

        [Required]
        public int StudentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public Teams AssignedTeam { get; set; }
    }
}