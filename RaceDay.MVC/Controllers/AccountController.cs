
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");

                var response = await client.PostAsJsonAsync(
                    "api/Users/register", model);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] =
                        "Registration successful. Please log in.";

                    return RedirectToAction("Login");
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }
            catch
            {
                ViewBag.Error = "Could not connect to the RaceDay API.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");

                var response = await client.PostAsJsonAsync(
                    "api/Users/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Invalid email or password.";
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();

                using var document = JsonDocument.Parse(json);
                var user = document.RootElement;

                HttpContext.Session.SetInt32(
                    "UserId",
                    user.GetProperty("userId").GetInt32());

                HttpContext.Session.SetString(
                    "FirstName",
                    user.GetProperty("firstName").GetString() ?? "");

                HttpContext.Session.SetString(
                    "LastName",
                    user.GetProperty("lastName").GetString() ?? "");

                HttpContext.Session.SetString(
                    "Email",
                    user.GetProperty("email").GetString() ?? "");

                HttpContext.Session.SetString(
                    "UserRole",
                    user.GetProperty("role").GetString() ?? "");

                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                ViewBag.Error = "Could not connect to the RaceDay API.";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }

    public class RegisterViewModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "Participant";
        public string? PhoneNumber { get; set; }
    }

    public class LoginViewModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}

