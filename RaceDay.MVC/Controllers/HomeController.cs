
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("RaceDayAPI");

                var response = await client.GetAsync("api/Events");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    ViewBag.Events = JsonSerializer.Deserialize<JsonElement>(json);
                }
                else
                {
                    ViewBag.Events = Array.Empty<JsonElement>();
                    ViewBag.Message = "Events are currently unavailable.";
                }
            }
            catch
            {
                ViewBag.Events = Array.Empty<JsonElement>();
                ViewBag.Message = "Could not connect to RaceDay services.";
            }

            return View();
        }
    }
}
