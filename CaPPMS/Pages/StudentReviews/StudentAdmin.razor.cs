using CaPPMS.Data;

using System.Collections.Generic;

namespace CaPPMS.Pages.StudentReviews
{
    public partial class Admin
    {
        public List<Student> Students { get; set; }
        public Admin()
        {
            Students = new List<Student>();
        }
    }
}