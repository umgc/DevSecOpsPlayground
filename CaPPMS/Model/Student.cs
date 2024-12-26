using CaPPMS.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CaPPMS.Model
{
    public class Student
    {
        private long classId = -1;

        public Student() { }

        [Required]
        public long StudentId { get; set; } = default;

        [ColumnHeader]
        [DisplayName("First Name")]
        public string? FirstName { get; set; }

        [ColumnHeader]
        [DisplayName("Last Name")]
        public string? LastName { get; set; }

        [ColumnHeader]
        public string? Email { get; set; }

        [ColumnHeader]
        [DisplayName("Team")]
        public string? TeamName
        {
            get
            {
                return this.AssignedTeam.Name;
            }
            set
            {
                this.AssignedTeam.Name = value ?? string.Empty;
            }
        }

        public Team AssignedTeam { get; private set; } = new Team();

        public long TeamId
        {
            get
            {
                return this.AssignedTeam.TeamId;
            }
            set
            {
                this.AssignedTeam.TeamId = value;
            }
        }

        public long ClassId
        {
            get
            {
                return this.classId;
            }
            set
            {
                this.classId = value;
            }
        }

        public Student Clone()
        {
            Student student = (Student)this.MemberwiseClone();
            Team team = new Team
            {
                TeamId = this.AssignedTeam.TeamId,
                Name = this.AssignedTeam.Name,
                ClassId = this.AssignedTeam.ClassId
            };
            student.AssignedTeam = team;
            return student;
        }
    }
}