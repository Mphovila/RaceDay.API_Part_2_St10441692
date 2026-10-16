
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("UserRole") == "Organiser"
                && HttpContext.Session.GetInt32("UserId").HasValue;
        }

        private async Task<List<EventOption>> GetEvents()
        {
            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            return await client.GetFromJsonAsync<List<EventOption>>("api/Events")
                   ?? new List<EventOption>();
        }

        // GET: /Categories
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var response = await client.GetAsync("api/Categories");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load categories from the API.";
                return View(new List<CategoryViewModel>());
            }

            var categories =
                await response.Content.ReadFromJsonAsync<List<CategoryViewModel>>();

            return View(categories ?? new List<CategoryViewModel>());
        }

        // GET: /Categories/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            ViewBag.Events = await GetEvents();

            return View(new CategoryViewModel());
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            ViewBag.Events = await GetEvents();

            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var response = await client.PostAsJsonAsync("api/Categories", new
            {
                model.EventId,
                model.CategoryName,
                model.Distance,
                model.EntryFee,
                model.MaximumParticipants
            });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.Error = error;
                return View(model);
            }

            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal Distance { get; set; }
        public decimal EntryFee { get; set; }
        public int MaximumParticipants { get; set; }
    }

    public class EventOption
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = "";
    }
}
