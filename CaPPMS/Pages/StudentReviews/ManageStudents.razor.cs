using CaPPMS.Data;
using System.Collections.Generic;

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
            return DBOperationsService.RetrieveTeamList();
        }

        public void UpdateTeamAssignments()
        {
            foreach (var student in Students)
            {
                DBOperationsService.UpdateTeamAssignment(student.StudentId, student.AssignedTeam.TeamId);
            }
        }

        public bool UpdateTeamMembers(SelectedStudent student)
        {
            return DBOperationsService.UpdateTeamAssignment(student.StudentId, student.AssignedTeam?.TeamId ?? -1);
        }
    }
}