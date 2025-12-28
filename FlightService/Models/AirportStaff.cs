using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightService.Models;

[Table("AirportStaff")]
public class AirportStaff
{
    [Key]
    [Column("staff_id")]
    public int StaffId { get; set; }

    [Required]
    [Column("username")]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("first_name")]
    [MaxLength(50)]
    public string? FirstName { get; set; }

    [Column("last_name")]
    [MaxLength(50)]
    public string? LastName { get; set; }

    [Required]
    [Column("role")]
    [MaxLength(20)]
    public string Role { get; set; } = "Operator"; // Admin, Manager, Operator, Viewer

    [Column("airport_id")]
    public int? AirportId { get; set; }

    [Column("airline_id")]
    public int? AirlineId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    [Column("failed_login_attempts")]
    public int FailedLoginAttempts { get; set; } = 0;

    [Column("locked_until")]
    public DateTime? LockedUntil { get; set; }

    [Column("password_reset_token")]
    [MaxLength(255)]
    public string? PasswordResetToken { get; set; }

    [Column("password_reset_expires")]
    public DateTime? PasswordResetExpires { get; set; }

    // Navigation properties
    [ForeignKey("AirportId")]
    public Airport? Airport { get; set; }

    [ForeignKey("AirlineId")]
    public Airline? Airline { get; set; }
}
