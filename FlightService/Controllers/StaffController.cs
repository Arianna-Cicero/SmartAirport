using FlightService.Data;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace FlightService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly FlightServiceDbContext _context;
    private readonly ILogger<StaffController> _logger;

    public StaffController(FlightServiceDbContext context, ILogger<StaffController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all staff members (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<StaffReadDto>>> GetAllStaff()
    {
        var staff = await _context.AirportStaff
            .Include(s => s.Airport)
            .Include(s => s.Airline)
            .Select(s => new StaffReadDto
            {
                StaffId = s.StaffId,
                Username = s.Username,
                Email = s.Email,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Role = s.Role,
                AirportId = s.AirportId,
                AirlineId = s.AirlineId,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                LastLogin = s.LastLogin
            })
            .ToListAsync();

        return Ok(staff);
    }

    /// <summary>
    /// Get staff member by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StaffReadDto>> GetStaffById(int id)
    {
        var staff = await _context.AirportStaff
            .Include(s => s.Airport)
            .Include(s => s.Airline)
            .FirstOrDefaultAsync(s => s.StaffId == id);

        if (staff == null)
        {
            return NotFound(new { message = "Staff member not found" });
        }

        // Users can only view their own profile unless they're admin
        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        if (currentUserId != id && currentUserRole != "Admin")
        {
            return Forbid();
        }

        var staffDto = new StaffReadDto
        {
            StaffId = staff.StaffId,
            Username = staff.Username,
            Email = staff.Email,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Role = staff.Role,
            AirportId = staff.AirportId,
            AirlineId = staff.AirlineId,
            IsActive = staff.IsActive,
            CreatedAt = staff.CreatedAt,
            LastLogin = staff.LastLogin
        };

        return Ok(staffDto);
    }

    /// <summary>
    /// Create new staff member (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StaffReadDto>> CreateStaff([FromBody] StaffRegisterDto staffDto)
    {
        // Check if username already exists
        if (await _context.AirportStaff.AnyAsync(s => s.Username == staffDto.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Check if email already exists
        if (await _context.AirportStaff.AnyAsync(s => s.Email == staffDto.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Validate airport and airline if provided
        if (staffDto.AirportId.HasValue)
        {
            var airportExists = await _context.Airports.AnyAsync(a => a.airport_id == staffDto.AirportId.Value);
            if (!airportExists)
            {
                return BadRequest(new { message = "Invalid airport ID" });
            }
        }

        if (staffDto.AirlineId.HasValue)
        {
            var airlineExists = await _context.Airlines.AnyAsync(a => a.airline_id == staffDto.AirlineId.Value);
            if (!airlineExists)
            {
                return BadRequest(new { message = "Invalid airline ID" });
            }
        }

        var staff = new AirportStaff
        {
            Username = staffDto.Username,
            Email = staffDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(staffDto.Password),
            FirstName = staffDto.FirstName,
            LastName = staffDto.LastName,
            Role = staffDto.Role,
            AirportId = staffDto.AirportId,
            AirlineId = staffDto.AirlineId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.AirportStaff.Add(staff);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Staff member created: {Username} with role {Role}", staff.Username, staff.Role);

        var result = new StaffReadDto
        {
            StaffId = staff.StaffId,
            Username = staff.Username,
            Email = staff.Email,
            FirstName = staff.FirstName,
            LastName = staff.LastName,
            Role = staff.Role,
            AirportId = staff.AirportId,
            AirlineId = staff.AirlineId,
            IsActive = staff.IsActive,
            CreatedAt = staff.CreatedAt,
            LastLogin = staff.LastLogin
        };

        return CreatedAtAction(nameof(GetStaffById), new { id = staff.StaffId }, result);
    }

    /// <summary>
    /// Update staff member
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaff(int id, [FromBody] StaffUpdateDto staffDto)
    {
        var staff = await _context.AirportStaff.FindAsync(id);
        if (staff == null)
        {
            return NotFound(new { message = "Staff member not found" });
        }

        // Users can only update their own profile unless they're admin
        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        if (currentUserId != id && currentUserRole != "Admin")
        {
            return Forbid();
        }

        // Only admins can change roles and active status
        if (currentUserRole != "Admin")
        {
            staffDto.Role = null;
            staffDto.IsActive = null;
        }

        if (!string.IsNullOrEmpty(staffDto.Email))
        {
            // Check if email is already taken by another user
            if (await _context.AirportStaff.AnyAsync(s => s.Email == staffDto.Email && s.StaffId != id))
            {
                return BadRequest(new { message = "Email already exists" });
            }
            staff.Email = staffDto.Email;
        }

        if (staffDto.FirstName != null) staff.FirstName = staffDto.FirstName;
        if (staffDto.LastName != null) staff.LastName = staffDto.LastName;
        if (staffDto.Role != null && currentUserRole == "Admin") staff.Role = staffDto.Role;
        if (staffDto.AirportId.HasValue) staff.AirportId = staffDto.AirportId;
        if (staffDto.AirlineId.HasValue) staff.AirlineId = staffDto.AirlineId;
        if (staffDto.IsActive.HasValue && currentUserRole == "Admin") staff.IsActive = staffDto.IsActive.Value;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Staff member updated: {StaffId}", id);

        return NoContent();
    }

    /// <summary>
    /// Delete staff member (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        var staff = await _context.AirportStaff.FindAsync(id);
        if (staff == null)
        {
            return NotFound(new { message = "Staff member not found" });
        }

        // Soft delete - just deactivate
        staff.IsActive = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Staff member deactivated: {StaffId}", id);

        return NoContent();
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto passwordDto)
    {
        var staff = await _context.AirportStaff.FindAsync(id);
        if (staff == null)
        {
            return NotFound(new { message = "Staff member not found" });
        }

        // Users can only change their own password
        var currentUserId = GetCurrentUserId();
        if (currentUserId != id)
        {
            return Forbid();
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(passwordDto.CurrentPassword, staff.PasswordHash))
        {
            return BadRequest(new { message = "Current password is incorrect" });
        }

        // Update password
        staff.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordDto.NewPassword);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Password changed for staff: {StaffId}", id);

        return Ok(new { message = "Password changed successfully" });
    }

    // Helper methods
    private int GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StaffId");
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    private string GetCurrentUserRole()
    {
        var roleClaim = User.Claims.FirstOrDefault(c => c.Type == "Role");
        return roleClaim?.Value ?? "";
    }
}
