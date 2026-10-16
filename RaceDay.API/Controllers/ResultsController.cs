
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public ResultsController(RaceDayContext context)
        {
            _context = context;
        }

        // GET: api/Results
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var results = await _context.Results
                .Select(r => new
                {
                    r.ResultId,
                    r.EnrolmentId,
                    r.FinishTime,
                    r.Position,
                    r.ResultStatus
                })
                .ToListAsync();

            return Ok(results);
        }

        // POST: api/Results
        [HttpPost]
        public async Task<IActionResult> CreateResult(
            [FromBody] CreateResultRequest request)
        {
            var enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentId == request.EnrolmentId);

            if (!enrolmentExists)
            {
                return BadRequest("The specified enrolment does not exist.");
            }

            var resultExists = await _context.Results
                .AnyAsync(r => r.EnrolmentId == request.EnrolmentId);

            if (resultExists)
            {
                return BadRequest(
                    "A result already exists for this enrolment.");
            }

            if (request.Position.HasValue && request.Position.Value <= 0)
            {
                return BadRequest("Position must be greater than zero.");
            }

            var result = new Result
            {
                EnrolmentId = request.EnrolmentId,
                FinishTime = request.FinishTime,
                Position = request.Position,
                ResultStatus = request.ResultStatus
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                result.ResultId,
                result.EnrolmentId,
                result.FinishTime,
                result.Position,
                result.ResultStatus
            });
        }
    }

    public class CreateResultRequest
    {
        public int EnrolmentId { get; set; }
        public string? FinishTime { get; set; }
        public int? Position { get; set; }
        public string ResultStatus { get; set; } = "Completed";
    }
}
