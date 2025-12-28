using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FlightService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Airlines",
                columns: table => new
                {
                    airline_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    iata = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    airlinename = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    base_airport = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airlines", x => x.airline_id);
                });

            migrationBuilder.CreateTable(
                name: "Airplanes",
                columns: table => new
                {
                    airplane_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    airline_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airplanes", x => x.airplane_id);
                });

            migrationBuilder.CreateTable(
                name: "AirportGeos",
                columns: table => new
                {
                    airport_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirportGeos", x => x.airport_id);
                });

            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    airport_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    iata = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    icao = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.airport_id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    flight_id = table.Column<int>(type: "integer", nullable: false),
                    passenger_id = table.Column<int>(type: "integer", nullable: false),
                    seat = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.booking_id);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    flight_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    flightno = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    from = table.Column<int>(type: "integer", nullable: false),
                    to = table.Column<int>(type: "integer", nullable: false),
                    departure = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    arrival = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    airline_id = table.Column<int>(type: "integer", nullable: false),
                    airplane_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.flight_id);
                });

            migrationBuilder.CreateTable(
                name: "FlightSchedules",
                columns: table => new
                {
                    flightno = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    from = table.Column<int>(type: "integer", nullable: false),
                    to = table.Column<int>(type: "integer", nullable: false),
                    departure = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    arrival = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    airline_id = table.Column<int>(type: "integer", nullable: false),
                    monday = table.Column<int>(type: "integer", nullable: false),
                    tuesday = table.Column<int>(type: "integer", nullable: false),
                    wednesday = table.Column<int>(type: "integer", nullable: false),
                    thursday = table.Column<int>(type: "integer", nullable: false),
                    friday = table.Column<int>(type: "integer", nullable: false),
                    saturday = table.Column<int>(type: "integer", nullable: false),
                    sunday = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSchedules", x => x.flightno);
                });

            migrationBuilder.CreateTable(
                name: "Passengers",
                columns: table => new
                {
                    passenger_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    firstname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    lastname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Passportno = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passengers", x => x.passenger_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Airlines");

            migrationBuilder.DropTable(
                name: "Airplanes");

            migrationBuilder.DropTable(
                name: "AirportGeos");

            migrationBuilder.DropTable(
                name: "Airports");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "FlightSchedules");

            migrationBuilder.DropTable(
                name: "Passengers");
        }
    }
}
