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
        public DbSet<AirportStaff> AirportStaff { get; set; }

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
            modelBuilder.Entity<AirportStaff>().HasKey(e => e.StaffId);

            // Indexes for better performance
            modelBuilder.Entity<AirportStaff>()
                .HasIndex(e => e.Username)
                .IsUnique();
            
            modelBuilder.Entity<AirportStaff>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.FromAirport)
                .WithMany()
                .HasForeignKey(f => f.from)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.ToAirport)
                .WithMany()
                .HasForeignKey(f => f.to)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FlightSchedule>()
                .HasOne(fs => fs.FromAirport)
                .WithMany()
                .HasForeignKey(fs => fs.from);

            modelBuilder.Entity<FlightSchedule>()
                .HasOne(fs => fs.ToAirport)
                .WithMany()
                .HasForeignKey(fs => fs.to);

            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.PassengerDetails)
                .WithOne(d => d.Passenger)
                .HasForeignKey<Passenger_details>(d => d.passenger_id);
        }
    }
}