
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace RaceDay.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EventsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetInt32("UserId") != null
                && HttpContext.Session.GetString("UserRole") == "Organiser";
        }

        // GET: /Events
        public async Task<IActionResult> Index()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try
            {
                var events = await client.GetFromJsonAsync<List<EventViewModel>>(
                    "api/Events");

                return View(events ?? new List<EventViewModel>());
            }
            catch
            {
                ViewBag.Error = "Could not load events from the API.";
                return View(new List<EventViewModel>());
            }
        }

        // GET: /Events/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            return View(new EventViewModel
            {
                EventDate = DateTime.Now.AddDays(7)
            });
        }

        // POST: /Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var request = new
            {
                OrganiserId = organiserId,
                model.EventName,
                model.Description,
                model.EventType,
                model.EventDate,
                model.Location,
                model.EntryFee
            };

            try
            {
                var response = await client.PostAsJsonAsync(
                    "api/Events", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Event created successfully!";
                    return RedirectToAction("Index");
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }
            catch
            {
                ViewBag.Error = "Could not connect to the API.";
                return View(model);
            }
        }

        // GET: /Events/Edit/1
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try
            {
                var eventItem = await client.GetFromJsonAsync<EventViewModel>(
                    $"api/Events/{id}");

                if (eventItem == null)
                    return NotFound();

                var organiserId = HttpContext.Session.GetInt32("UserId");

                if (eventItem.OrganiserId != organiserId)
                    return Forbid();

                return View(eventItem);
            }
            catch
            {
                return NotFound();
            }
        }

        // POST: /Events/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            var request = new
            {
                OrganiserId = organiserId,
                model.EventName,
                model.Description,
                model.EventType,
                model.EventDate,
                model.Location,
                model.EntryFee
            };

            try
            {
                var response = await client.PutAsJsonAsync(
                    $"api/Events/{model.EventId}", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Event updated successfully!";
                    return RedirectToAction("Index");
                }

                ViewBag.Error = await response.Content.ReadAsStringAsync();
                return View(model);
            }
            catch
            {
                ViewBag.Error = "Could not connect to the API.";
                return View(model);
            }
        }

        // POST: /Events/Delete/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Account");

            var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;

            var client = _httpClientFactory.CreateClient("RaceDayAPI");

            try
            {
                var response = await client.DeleteAsync(
                    $"api/Events/{id}?organiserId={organiserId}");

                if (response.IsSuccessStatusCode)
                    TempData["Success"] = "Event deleted successfully!";
                else
                    TempData["Error"] = "Could not delete this event.";
            }
            catch
            {
                TempData["Error"] = "Could not connect to the API.";
            }

            return RedirectToAction("Index");
        }
    }

    public class EventViewModel
    {
        public int EventId { get; set; }

        public int OrganiserId { get; set; }

        public string EventName { get; set; } = "";

        public string? Description { get; set; }

        public string EventType { get; set; } = "";

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = "";

        public decimal EntryFee { get; set; }
    }
}
