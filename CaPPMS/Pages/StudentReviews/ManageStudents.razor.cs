using CaPPMS.Data;
using CaPPMS.Shared;
using System.Collections.Generic;
using System.Linq;

namespace CaPPMS.Pages.StudentReviews
{
    public partial class ManageStudents
    {
        public bool HidePanel { get; set; } = true;

        private List<Student> value = new List<Student>();

        public List<Student> Students
        {
            get
            {
                if (value != null && value.Count > 0)
                {
                    var tempTeam = teams.Find(e => e.TeamId == value[0].AssignedTeam.TeamId);

                    if (value.Count > 0 && tempTeam != null)
                    {
                        value[0].AssignedTeam.Name = tempTeam.Name;
                    }
                }
                else
                {
                    value = new List<Student>();
                }

                return value;
            }

            set { this.Students = value; }
        }

        public ManageStudents()
        {
            teams = new List<Teams>();
            selectedStudent = new SelectedStudent();
        }

        public List<Teams> RetrieveTeams()
        {
            return DBOperations.RetrieveTeamList();
        }

        public void UpdateTeamAssignments()
        {
            foreach (var student in Students)
            {
                DBOperations.UpdateTeamAssignment(student.StudentId, student.AssignedTeam.TeamId);
            }
        }

        public bool UpdateTeamMembers(SelectedStudent student)
        {
            return DBOperations.UpdateTeamAssignment(student.StudentId, student.AssignedTeam.TeamId);
        }
    }
}