using CaPPMS.Data;
using CaPPMS.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaPPMS.Pages.StudentReviews
{
    public partial class ManageStudents
    {
        private IEnumerable<Team>? teams;

        public bool HidePanel { get; set; } = true;

        public List<Student> Students { get; set; } = [];

        public ManageStudents()
        {
            selectedStudent = new SelectedStudent();
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