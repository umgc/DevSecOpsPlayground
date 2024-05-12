using CaPPMS.Shared;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Data
{
    public class Student
    {
        [Required]
        public int StudentId { get; set; } = default(int);

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public Teams AssignedTeam { get; set; } = new Teams();
    }
}