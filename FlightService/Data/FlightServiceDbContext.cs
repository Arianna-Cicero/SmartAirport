using FlightService.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightService.Data
{
    public class FlightServiceDbContext : DbContext
    {
        public FlightServiceDbContext(DbContextOptions<FlightServiceDbContext> options) : base(options)
        {
        }

        public DbSet<Airport> Airports { get; set; }
        public DbSet<AirportGeo> AirportGeos { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<FlightSchedule> FlightSchedules { get; set; }
        public DbSet<Airline> Airlines { get; set; }
        public DbSet<Airplane> Airplanes { get; set; }
        public DbSet<Passenger> Passengers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // primary keys
            modelBuilder.Entity<Flight>().HasKey(e => e.flight_id);
            modelBuilder.Entity<Booking>().HasKey(e => e.booking_id);
            modelBuilder.Entity<FlightSchedule>().HasKey(e => e.flightno);
            modelBuilder.Entity<Airline>().HasKey(e => e.airline_id);
            modelBuilder.Entity<Airplane>().HasKey(e => e.airplane_id);
            modelBuilder.Entity<Passenger>().HasKey(e => e.passenger_id);
            modelBuilder.Entity<Airport>().HasKey(e => e.airport_id);
            modelBuilder.Entity<AirportGeo>().HasKey(e => e.airport_id);
        }
    }
}