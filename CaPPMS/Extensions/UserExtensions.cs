using Microsoft.Graph;
using System.Security.Claims;

namespace CaPPMS.Extensions
{
    public static class UserExtensions
    {
        public const string Admin = "c1e4f42f-3e63-4dd8-8810-c21f3b4e5894";
        public const string Student = "39be3a7b-a12e-4535-821c-9823181c6d21";
        private const string canInvite = "1142214c-0150-4043-8b20-de8e45d9b452";
        private const string canDelete = "072b7ca7-3933-4be6-ab9e-0455e9b14330";

        public static bool IsStudent(this ClaimsPrincipal user)
        {
            return user.HasClaim(c => string.Equals(Student, c.Value));
        }

        public static bool IsStudent(this User user)
        {
            DirectoryObject studentGroup = new DirectoryObject
            {
                Id = Student
            };
            return user.MemberOf != null && user.MemberOf.Contains(studentGroup);
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.HasClaim(c => string.Equals(Admin, c.Value));
        }

        public static bool HasStudentAreaAccess(this ClaimsPrincipal user)
        {
            return user.IsStudent() || user.IsAdmin();
        }

        public static bool CanInvite(this ClaimsPrincipal user)
        {
            return user.HasClaim(c => string.Equals(canInvite, c.Value));
        }

        public static bool CanDelete(this ClaimsPrincipal user)
        {
            return user.HasClaim(c => string.Equals(canDelete, c.Value));
        }
    }
}
