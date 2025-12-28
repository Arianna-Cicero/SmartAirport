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

            // Airport relations (1:1 with same primary key)
            modelBuilder.Entity<Airport>()
                .HasOne(a => a.AirportGeo)
                .WithOne()
                .HasForeignKey<AirportGeo>(ag => ag.airport_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Airport>()
                .HasOne(a => a.AirportReachable)
                .WithOne()
                .HasForeignKey<Airport_reachable>(ar => ar.airport_id)
                .OnDelete(DeleteBehavior.Cascade);

            // Flight relations
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

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Airline)
                .WithMany(a => a.Flights)
                .HasForeignKey(f => f.airline_id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Airplane)
                .WithMany()
                .HasForeignKey(f => f.airplane_id)
                .OnDelete(DeleteBehavior.Restrict);

            // FlightSchedule relations
            modelBuilder.Entity<FlightSchedule>()
                .HasOne(fs => fs.FromAirport)
                .WithMany()
                .HasForeignKey(fs => fs.from)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FlightSchedule>()
                .HasOne(fs => fs.ToAirport)
                .WithMany()
                .HasForeignKey(fs => fs.to)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FlightSchedule>()
                .HasOne(fs => fs.Airline)
                .WithMany()
                .HasForeignKey(fs => fs.airline_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Passenger relations
            modelBuilder.Entity<Passenger>()
                .HasOne(p => p.PassengerDetails)
                .WithOne(d => d.Passenger)
                .HasForeignKey<Passenger_details>(d => d.passenger_id);

            // Booking relations
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Flight)
                .WithMany()
                .HasForeignKey(b => b.flight_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Passenger)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.passenger_id)
                .OnDelete(DeleteBehavior.Cascade);

            // Airplane relations
            modelBuilder.Entity<Airplane>()
                 .HasOne(a => a.Airline)
                 .WithMany(al => al.Airplanes)
                 .HasForeignKey(a => a.airline_id)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Airplane>()
                .HasOne(a => a.AirplaneType)
                .WithMany()
                .HasForeignKey(a => a.type_id)
                .OnDelete(DeleteBehavior.Restrict);

            // Airline relations
            modelBuilder.Entity<Airline>()
                .HasOne(a => a.BaseAirport)
                .WithMany()
                .HasForeignKey(a => a.base_airport)
                .OnDelete(DeleteBehavior.Restrict);

            // AirportStaff relations
            modelBuilder.Entity<AirportStaff>()
                .HasOne(s => s.Airport)
                .WithMany()
                .HasForeignKey(s => s.AirportId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AirportStaff>()
                .HasOne(s => s.Airline)
                .WithMany()
                .HasForeignKey(s => s.AirlineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}