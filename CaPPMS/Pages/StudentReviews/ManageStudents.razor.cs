using CaPPMS.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task UpdateTeamAssignmentsAsync()
        {
            foreach (var student in Students)
            {
                await DBOperationsService.UpdateTeamAssignmentAsync(student.StudentId, student.AssignedTeam.TeamId);
            }
        }

        public async Task<bool> UpdateTeamMembersAsync(SelectedStudent student)
        {
            return await DBOperationsService.UpdateTeamAssignmentAsync(student.StudentId, student.AssignedTeam?.TeamId ?? -1);
        }
    }
}