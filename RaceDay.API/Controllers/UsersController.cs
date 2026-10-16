
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UsersController(RaceDayContext context)
        {
            _context = context;
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("All required fields must be provided.");
            }

            if (request.Password.Length < 8)
            {
                return BadRequest(
                    "Password must contain at least 8 characters.");
            }

            if (request.Role != "Participant" &&
                request.Role != "Organiser")
            {
                return BadRequest(
                    "Role must be Participant or Organiser.");
            }

            var email = request.Email.Trim();

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (emailExists)
            {
                return Conflict("An account with this email already exists.");
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Role = request.Role,
                PhoneNumber = request.PhoneNumber,
                CreatedAt = DateTime.Now
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role,
                user.PhoneNumber,
                user.CreatedAt
            });
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.Trim());

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var verification = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserRole", user.Role);

            return Ok(new
            {
                message = "Login successful.",
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }

        // POST: api/Users/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }

        // GET: api/Users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            var user = await _context.Users
                .Where(u => u.UserId == userId.Value)
                .Select(u => new
                {
                    u.UserId,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role,
                    u.PhoneNumber,
                    u.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                HttpContext.Session.Clear();
                return Unauthorized("User account was not found.");
            }

            return Ok(user);
        }
    }

    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Participant";
        public string? PhoneNumber { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
