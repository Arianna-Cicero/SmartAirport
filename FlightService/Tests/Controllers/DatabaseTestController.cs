using FlightService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Tests.Controllers;

#if DEBUG
[ApiController]
[Route("api/[controller]")]
public class DatabaseTestController : ControllerBase
{
    private readonly FlightServiceDbContext _context;
    private readonly ILogger<DatabaseTestController> _logger;

    public DatabaseTestController(
        FlightServiceDbContext context,
        ILogger<DatabaseTestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Test database connection with detailed diagnostics
    /// </summary>
    [HttpGet("connection")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            _logger.LogInformation("Starting database connection test...");
            
            // Try to connect to database
            _logger.LogInformation("Attempting to connect to database...");
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (canConnect)
            {
                _logger.LogInformation("Database connection successful!");
                return Ok(new
                {
                    status = "success",
                    message = "Database connection is working!",
                    timestamp = DateTime.UtcNow
                });
            }
            else
            {
                _logger.LogWarning("Database connection failed - CanConnectAsync returned false");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Cannot connect to database",
                    timestamp = DateTime.UtcNow
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing database connection");
            
            // Don't expose sensitive connection details in production
            var errorDetails = new
            {
                status = "error",
                message = "Database connection failed",
                timestamp = DateTime.UtcNow
            };
            
            return StatusCode(500, errorDetails);
        }
    }

    /// <summary>
    /// Get database information
    /// </summary>
    [HttpGet("info")]
    public async Task<IActionResult> GetDatabaseInfo()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return StatusCode(500, new { message = "Cannot connect to database" });
            }

            // Get connection string (without password for security)
            var connectionString = _context.Database.GetConnectionString();
            var sanitizedConnection = SanitizeConnectionString(connectionString);

            return Ok(new
            {
                status = "connected",
                provider = _context.Database.ProviderName,
                connectionString = sanitizedConnection,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database info");
            return StatusCode(500, new
            {
                status = "error",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Check pending migrations
    /// </summary>
    [HttpGet("migrations/pending")]
    public async Task<IActionResult> GetPendingMigrations()
    {
        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

            return Ok(new
            {
                hasPendingMigrations = pendingMigrations.Any(),
                pendingMigrations = pendingMigrations.ToList(),
                appliedMigrations = appliedMigrations.ToList(),
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking migrations");
            return StatusCode(500, new
            {
                status = "error",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// List all tables in the database
    /// </summary>
    [HttpGet("tables")]
    public async Task<IActionResult> GetTables()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            
            if (!canConnect)
            {
                return StatusCode(500, new { message = "Cannot connect to database" });
            }

            // Query to count tables in PostgreSQL
            var sql = @"
                SELECT COUNT(*) 
                FROM information_schema.tables 
                WHERE table_schema = 'public';
            ";

            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            
            var command = connection.CreateCommand();
            command.CommandText = sql;
            
            var tableCount = Convert.ToInt32(await command.ExecuteScalarAsync());
            
            await connection.CloseAsync();

            return Ok(new
            {
                status = "success",
                tableCount = tableCount,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting database tables");
            return StatusCode(500, new
            {
                status = "error",
                message = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    private string SanitizeConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "No connection string";

        // Remove password from connection string for security
        var parts = connectionString.Split(';');
        var sanitized = parts
            .Where(p => !p.Trim().StartsWith("Password", StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Trim().StartsWith("Username", StringComparison.OrdinalIgnoreCase) 
                ? "Username=***" 
                : p);

        return string.Join("; ", sanitized);
    }
}
#endif
