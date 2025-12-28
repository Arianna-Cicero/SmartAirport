using FlightService.Data;
using FlightService.DTOs;
using FlightService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace FlightService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly FlightServiceDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ITokenService tokenService, 
            FlightServiceDbContext context,
            ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Login with username and password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] StaffLoginDto request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            // Find staff member by username
            var staff = await _context.AirportStaff
                .FirstOrDefaultAsync(s => s.Username == request.Username);

            if (staff == null)
            {
                _logger.LogWarning("Login attempt with invalid username: {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Check if account is locked
            if (staff.LockedUntil.HasValue && staff.LockedUntil.Value > DateTime.UtcNow)
            {
                _logger.LogWarning("Login attempt for locked account: {Username}", request.Username);
                return Unauthorized(new { message = $"Account is locked until {staff.LockedUntil.Value:yyyy-MM-dd HH:mm:ss} UTC" });
            }

            // Check if account is active
            if (!staff.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive account: {Username}", request.Username);
                return Unauthorized(new { message = "Account is inactive" });
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, staff.PasswordHash))
            {
                // Increment failed login attempts
                staff.FailedLoginAttempts++;

                // Lock account after 5 failed attempts for 15 minutes
                if (staff.FailedLoginAttempts >= 5)
                {
                    staff.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                    _logger.LogWarning("Account locked due to failed login attempts: {Username}", request.Username);
                }

                await _context.SaveChangesAsync();

                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Reset failed login attempts and update last login
            staff.FailedLoginAttempts = 0;
            staff.LockedUntil = null;
            staff.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Generate JWT token with custom claims
            var token = _tokenService.GenerateToken(
                staff.Username, 
                new[] { staff.Role },
                new Dictionary<string, string>
                {
                    { "StaffId", staff.StaffId.ToString() },
                    { "Email", staff.Email },
                    { "Role", staff.Role }
                });

            _logger.LogInformation("Successful login: {Username} (StaffId: {StaffId})", staff.Username, staff.StaffId);

            return Ok(new
            {
                token,
                user = new
                {
                    staffId = staff.StaffId,
                    username = staff.Username,
                    email = staff.Email,
                    firstName = staff.FirstName,
                    lastName = staff.LastName,
                    role = staff.Role,
                    airportId = staff.AirportId,
                    airlineId = staff.AirlineId
                }
            });
        }

        /// <summary>
        /// Register new staff member (public registration - Admin approval required)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] StaffRegisterDto request)
        {
            // Check if username already exists
            if (await _context.AirportStaff.AnyAsync(s => s.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Check if email already exists
            if (await _context.AirportStaff.AnyAsync(s => s.Email == request.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            var staff = new FlightService.Models.AirportStaff
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = "Operator", // Default role, can be changed by Admin
                AirportId = request.AirportId,
                AirlineId = request.AirlineId,
                IsActive = false, // Requires admin activation
                CreatedAt = DateTime.UtcNow
            };

            _context.AirportStaff.Add(staff);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New staff registered: {Username} - Requires admin activation", staff.Username);

            return Ok(new 
            { 
                message = "Registration successful. Your account requires admin activation before you can login.",
                staffId = staff.StaffId 
            });
        }

        /// <summary>
        /// Get current user info
        /// </summary>
        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StaffId");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var staffId = int.Parse(userIdClaim.Value);
            var staff = await _context.AirportStaff
                .Include(s => s.Airport)
                .Include(s => s.Airline)
                .FirstOrDefaultAsync(s => s.StaffId == staffId);

            if (staff == null)
            {
                return NotFound(new { message = "Staff member not found" });
            }

            return Ok(new
            {
                staffId = staff.StaffId,
                username = staff.Username,
                email = staff.Email,
                firstName = staff.FirstName,
                lastName = staff.LastName,
                role = staff.Role,
                airportId = staff.AirportId,
                airportName = staff.Airport?.name,
                airlineId = staff.AirlineId,
                airlineName = staff.Airline?.airlinename,
                isActive = staff.IsActive,
                createdAt = staff.CreatedAt,
                lastLogin = staff.LastLogin
            });
        }
    }
}
