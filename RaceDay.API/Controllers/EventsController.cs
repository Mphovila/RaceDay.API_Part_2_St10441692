
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EventsController(RaceDayContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .Select(e => new
                {
                    e.EventId,
                    e.EventName,
                    e.Description,
                    e.EventType,
                    e.EventDate,
                    e.Location,
                    e.EntryFee
                })
                .ToListAsync();

            return Ok(events);
        }

        // POST: api/Events
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.EventName) ||
                string.IsNullOrWhiteSpace(request.EventType) ||
                string.IsNullOrWhiteSpace(request.Location))
            {
                return BadRequest(
                    "Event name, event type and location are required.");
            }

            if (request.EntryFee < 0)
            {
                return BadRequest("Entry fee cannot be negative.");
            }

            var organiserExists = await _context.Users
                .AnyAsync(u => u.UserId == request.OrganiserId);

            if (!organiserExists)
            {
                return BadRequest(
                    "The specified organiser does not exist.");
            }

            var newEvent = new RaceDay.API.Models.Event
            {
                OrganiserId = request.OrganiserId,
                EventName = request.EventName,
                Description = request.Description,
                EventType = request.EventType,
                EventDate = request.EventDate,
                Location = request.Location,
                EntryFee = request.EntryFee
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvents),
                new { id = newEvent.EventId },
                new
                {
                    newEvent.EventId,
                    newEvent.EventName,
                    newEvent.Description,
                    newEvent.EventType,
                    newEvent.EventDate,
                    newEvent.Location,
                    newEvent.EntryFee
                });
        }
    }

    public class CreateEventRequest
    {
        public int OrganiserId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string EventType { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal EntryFee { get; set; }
    }
}

