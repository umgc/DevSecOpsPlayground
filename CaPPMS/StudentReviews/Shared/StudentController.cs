using Microsoft.AspNetCore.Mvc;

namespace StudentReviews.Shared
{
    public class StudentController : Controller
    {
        public ActionResult Index()
        {
            var students = DBOperations.RetrieveStudents();
            var availableTeams = DBOperations.RetrieveTeamList();

            ViewBag.AvailableTeams = availableTeams;

            return View(students);
        }
    }
}