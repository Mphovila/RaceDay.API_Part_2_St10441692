
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EnrolmentsController(RaceDayContext context)
        {
            _context = context;
        }

        // GET: api/Enrolments
        [HttpGet]
        public async Task<IActionResult> GetEnrolments()
        {
            var enrolments = await _context.Enrolments
                .Select(e => new
                {
                    e.EnrolmentId,
                    e.ParticipantId,
                    e.EventId,
                    e.CategoryId,
                    e.EnrolmentDate,
                    e.Status
                })
                .ToListAsync();

            return Ok(enrolments);
        }

        // POST: api/Enrolments
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment(
            [FromBody] CreateEnrolmentRequest request)
        {
            var participantExists = await _context.Users
                .AnyAsync(u => u.UserId == request.ParticipantId);

            if (!participantExists)
            {
                return BadRequest("The specified participant does not exist.");
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == request.CategoryId &&
                    c.EventId == request.EventId);

            if (category == null)
            {
                return BadRequest(
                    "The specified category does not belong to this event.");
            }

            var alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e =>
                    e.ParticipantId == request.ParticipantId &&
                    e.EventId == request.EventId &&
                    e.CategoryId == request.CategoryId);

            if (alreadyEnrolled)
            {
                return BadRequest(
                    "This participant is already enrolled in this category.");
            }

            var enrolment = new Enrolment
            {
                ParticipantId = request.ParticipantId,
                EventId = request.EventId,
                CategoryId = request.CategoryId,
                EnrolmentDate = DateTime.Now,
                Status = "Active"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                enrolment.EnrolmentId,
                enrolment.ParticipantId,
                enrolment.EventId,
                enrolment.CategoryId,
                enrolment.EnrolmentDate,
                enrolment.Status
            });
        }
    }

    public class CreateEnrolmentRequest
    {
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
    }
}
