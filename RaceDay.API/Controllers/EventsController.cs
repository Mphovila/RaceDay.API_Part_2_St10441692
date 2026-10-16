
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

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
                    e.OrganiserId,
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

        // GET: api/Events/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Where(e => e.EventId == id)
                .Select(e => new
                {
                    e.EventId,
                    e.OrganiserId,
                    e.EventName,
                    e.Description,
                    e.EventType,
                    e.EventDate,
                    e.Location,
                    e.EntryFee
                })
                .FirstOrDefaultAsync();

            if (eventItem == null)
                return NotFound("Event not found.");

            return Ok(eventItem);
        }

        // POST: api/Events
        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] CreateEventRequest request)
        {
            if (!IsValid(request.EventName, request.EventType,
                    request.Location, request.EntryFee))
            {
                return BadRequest(
                    "Please provide valid event details.");
            }

            var organiserExists = await _context.Users
                .AnyAsync(u => u.UserId == request.OrganiserId);

            if (!organiserExists)
                return BadRequest("The specified organiser does not exist.");

            var newEvent = new Event
            {
                OrganiserId = request.OrganiserId,
                EventName = request.EventName.Trim(),
                Description = request.Description,
                EventType = request.EventType.Trim(),
                EventDate = request.EventDate,
                Location = request.Location.Trim(),
                EntryFee = request.EntryFee
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = newEvent.EventId },
                newEvent);
        }

        // PUT: api/Events/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] UpdateEventRequest request)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null)
                return NotFound("Event not found.");

            if (eventItem.OrganiserId != request.OrganiserId)
                return Forbid();

            if (!IsValid(request.EventName, request.EventType,
                    request.Location, request.EntryFee))
            {
                return BadRequest(
                    "Please provide valid event details.");
            }

            eventItem.EventName = request.EventName.Trim();
            eventItem.Description = request.Description;
            eventItem.EventType = request.EventType.Trim();
            eventItem.EventDate = request.EventDate;
            eventItem.Location = request.Location.Trim();
            eventItem.EntryFee = request.EntryFee;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event updated successfully.",
                eventItem.EventId
            });
        }

        // DELETE: api/Events/1?organiserId=1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(
            int id,
            [FromQuery] int organiserId)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (eventItem == null)
                return NotFound("Event not found.");

            if (eventItem.OrganiserId != organiserId)
                return Forbid();

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Event deleted successfully."
            });
        }

        private static bool IsValid(
            string? eventName,
            string? eventType,
            string? location,
            decimal entryFee)
        {
            return !string.IsNullOrWhiteSpace(eventName)
                && !string.IsNullOrWhiteSpace(eventType)
                && !string.IsNullOrWhiteSpace(location)
                && entryFee >= 0;
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

    public class UpdateEventRequest
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
