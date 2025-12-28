using FlightService.Data;
using FlightService.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Data;

public static class DbInitializer
{
    public static async Task SeedData(FlightServiceDbContext context, ILogger logger)
    {
        try
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Check if we already have staff
            if (await context.AirportStaff.AnyAsync())
            {
                logger.LogInformation("Database already contains staff data. Skipping seed.");
                return;
            }

            logger.LogInformation("Seeding initial staff data...");

            // Create default Admin user
            var adminStaff = new AirportStaff
            {
                Username = "admin",
                Email = "admin@smartairport.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "System",
                LastName = "Administrator",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Create default Operator user
            var operatorStaff = new AirportStaff
            {
                Username = "operator",
                Email = "operator@smartairport.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Operator123!"),
                FirstName = "Test",
                LastName = "Operator",
                Role = "Operator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.AirportStaff.AddRange(adminStaff, operatorStaff);
            await context.SaveChangesAsync();

            logger.LogInformation("Staff data seeded successfully!");
            logger.LogInformation("Default Admin - Username: admin, Password: Admin123!");
            logger.LogInformation("Default Operator - Username: operator, Password: Operator123!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding staff data");
            throw;
        }
    }
}
