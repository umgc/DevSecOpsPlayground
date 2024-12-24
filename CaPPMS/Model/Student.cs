using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    public class Student
    {
        [Required]
        public long StudentId { get; set; } = default;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public Team AssignedTeam { get; set; } = new Team();
    }
}