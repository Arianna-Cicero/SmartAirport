namespace FlightService.DTOs;

public class StaffRegisterDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = "Operator";
    public int? AirportId { get; set; }
    public int? AirlineId { get; set; }
}

public class StaffLoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class StaffReadDto
{
    public int StaffId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Role { get; set; }
    public int? AirportId { get; set; }
    public int? AirlineId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}

public class StaffUpdateDto
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
    public int? AirportId { get; set; }
    public int? AirlineId { get; set; }
    public bool? IsActive { get; set; }
}

public class ChangePasswordDto
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
