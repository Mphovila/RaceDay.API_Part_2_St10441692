
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role == "Organiser")
            {
                return RedirectToAction("Organiser");
            }

            if (role == "Participant")
            {
                return RedirectToAction("Participant");
            }

            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }

        public IActionResult Participant()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("UserRole") != "Participant")
                return RedirectToAction("Index");

            SetUserDetails();
            return View();
        }

        public IActionResult Organiser()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("UserRole") != "Organiser")
                return RedirectToAction("Index");

            SetUserDetails();
            return View();
        }

        private void SetUserDetails()
        {
            ViewBag.FirstName =
                HttpContext.Session.GetString("FirstName");

            ViewBag.LastName =
                HttpContext.Session.GetString("LastName");

            ViewBag.Email =
                HttpContext.Session.GetString("Email");
        }
    }
}
