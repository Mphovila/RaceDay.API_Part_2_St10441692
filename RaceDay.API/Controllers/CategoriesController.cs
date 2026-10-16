
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public CategoriesController(RaceDayContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.CategoryId,
                    c.EventId,
                    c.CategoryName,
                    c.Distance,
                    c.EntryFee,
                    c.MaximumParticipants
                })
                .ToListAsync();

            return Ok(categories);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CategoryName))
            {
                return BadRequest("Category name is required.");
            }

            if (request.Distance <= 0 ||
                request.EntryFee < 0 ||
                request.MaximumParticipants <= 0)
            {
                return BadRequest(
                    "Distance and maximum participants must be positive, and entry fee cannot be negative.");
            }

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == request.EventId);

            if (!eventExists)
            {
                return BadRequest("The specified event does not exist.");
            }

            var category = new Category
            {
                EventId = request.EventId,
                CategoryName = request.CategoryName,
                Distance = request.Distance,
                EntryFee = request.EntryFee,
                MaximumParticipants = request.MaximumParticipants
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                category.CategoryId,
                category.EventId,
                category.CategoryName,
                category.Distance,
                category.EntryFee,
                category.MaximumParticipants
            });
        }
    }

    public class CreateCategoryRequest
    {
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public decimal EntryFee { get; set; }
        public int MaximumParticipants { get; set; }
    }
}

