using StudentReviews.Data;

namespace StudentReviews.Pages
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