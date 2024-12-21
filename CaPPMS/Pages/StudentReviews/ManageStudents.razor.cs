using CaPPMS.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaPPMS.Pages.StudentReviews
{
    public partial class ManageStudents
    {
        public bool HidePanel { get; set; } = true;

        public List<Student> Students { get; set; } = [];

        public ManageStudents()
        {
            teams = new List<Team>();
            selectedStudent = new SelectedStudent();
        }

        public async Task<IEnumerable<Team>> RetrieveTeamsAsync()
        {
            return await DBOperationsService.RetrieveTeamListAsync();
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