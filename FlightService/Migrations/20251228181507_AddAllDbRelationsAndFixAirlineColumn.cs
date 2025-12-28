using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FlightService.Migrations
{
    /// <inheritdoc />
    public partial class AddAllDbRelationsAndFixAirlineColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, rename the incorrect column name from airpline_id to airline_id if it exists
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns 
                        WHERE table_name = 'Airplanes' AND column_name = 'airpline_id'
                    ) THEN
                        ALTER TABLE ""Airplanes"" RENAME COLUMN airpline_id TO airline_id;
                    END IF;
                END $$;
            ");

            // Clean up invalid foreign key references before adding constraints
            // Delete airlines with invalid base_airport references
            migrationBuilder.Sql(@"
                DELETE FROM ""Airlines"" 
                WHERE NOT EXISTS (SELECT 1 FROM ""Airports"" WHERE airport_id = ""Airlines"".base_airport)
            ");

            // Delete airplanes with invalid references
            migrationBuilder.Sql(@"
                DELETE FROM ""Airplanes"" 
                WHERE NOT EXISTS (SELECT 1 FROM ""Airlines"" WHERE airline_id = ""Airplanes"".airline_id)
            ");

            // Delete flights with invalid references
            migrationBuilder.Sql(@"
                DELETE FROM ""Bookings"" 
                WHERE NOT EXISTS (SELECT 1 FROM ""Flights"" WHERE flight_id = ""Bookings"".flight_id)
                   OR NOT EXISTS (SELECT 1 FROM ""Passengers"" WHERE passenger_id = ""Bookings"".passenger_id)
            ");

            migrationBuilder.Sql(@"
                DELETE FROM ""Flights"" 
                WHERE NOT EXISTS (SELECT 1 FROM ""Airplanes"" WHERE airplane_id = ""Flights"".airplane_id)
                   OR NOT EXISTS (SELECT 1 FROM ""Airlines"" WHERE airline_id = ""Flights"".airline_id)
                   OR NOT EXISTS (SELECT 1 FROM ""Airports"" WHERE airport_id = ""Flights"".""from"")
                   OR NOT EXISTS (SELECT 1 FROM ""Airports"" WHERE airport_id = ""Flights"".""to"")
            ");

            // Delete flight schedules with invalid references
            migrationBuilder.Sql(@"
                DELETE FROM ""FlightSchedules"" 
                WHERE NOT EXISTS (SELECT 1 FROM ""Airlines"" WHERE airline_id = ""FlightSchedules"".airline_id)
                   OR NOT EXISTS (SELECT 1 FROM ""Airports"" WHERE airport_id = ""FlightSchedules"".""from"")
                   OR NOT EXISTS (SELECT 1 FROM ""Airports"" WHERE airport_id = ""FlightSchedules"".""to"")
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_AirportStaff_Airlines_airline_id",
                table: "AirportStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_AirportStaff_Airports_airport_id",
                table: "AirportStaff");

            migrationBuilder.AlterColumn<int>(
                name: "airport_id",
                table: "AirportGeos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateTable(
                name: "Airplane_type",
                columns: table => new
                {
                    type_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airplane_type", x => x.type_id);
                });

            migrationBuilder.CreateTable(
                name: "Airport_reachable",
                columns: table => new
                {
                    airport_id = table.Column<int>(type: "integer", nullable: false),
                    hops = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airport_reachable", x => x.airport_id);
                    table.ForeignKey(
                        name: "FK_Airport_reachable_Airports_airport_id",
                        column: x => x.airport_id,
                        principalTable: "Airports",
                        principalColumn: "airport_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passenger_details",
                columns: table => new
                {
                    passenger_id = table.Column<int>(type: "integer", nullable: false),
                    birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    sex = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    zip = table.Column<int>(type: "integer", nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    telephone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passenger_details", x => x.passenger_id);
                    table.ForeignKey(
                        name: "FK_Passenger_details_Passengers_passenger_id",
                        column: x => x.passenger_id,
                        principalTable: "Passengers",
                        principalColumn: "passenger_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_airline_id",
                table: "FlightSchedules",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_from",
                table: "FlightSchedules",
                column: "from");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_to",
                table: "FlightSchedules",
                column: "to");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_airline_id",
                table: "Flights",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_airplane_id",
                table: "Flights",
                column: "airplane_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_from",
                table: "Flights",
                column: "from");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_to",
                table: "Flights",
                column: "to");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_flight_id",
                table: "Bookings",
                column: "flight_id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_passenger_id",
                table: "Bookings",
                column: "passenger_id");

            migrationBuilder.CreateIndex(
                name: "IX_Airplanes_airline_id",
                table: "Airplanes",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "IX_Airplanes_type_id",
                table: "Airplanes",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Airlines_base_airport",
                table: "Airlines",
                column: "base_airport");

            migrationBuilder.AddForeignKey(
                name: "FK_Airlines_Airports_base_airport",
                table: "Airlines",
                column: "base_airport",
                principalTable: "Airports",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Airplanes_Airlines_airline_id",
                table: "Airplanes",
                column: "airline_id",
                principalTable: "Airlines",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Airplanes_Airplane_type_type_id",
                table: "Airplanes",
                column: "type_id",
                principalTable: "Airplane_type",
                principalColumn: "type_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AirportGeos_Airports_airport_id",
                table: "AirportGeos",
                column: "airport_id",
                principalTable: "Airports",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AirportStaff_Airlines_airline_id",
                table: "AirportStaff",
                column: "airline_id",
                principalTable: "Airlines",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AirportStaff_Airports_airport_id",
                table: "AirportStaff",
                column: "airport_id",
                principalTable: "Airports",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Flights_flight_id",
                table: "Bookings",
                column: "flight_id",
                principalTable: "Flights",
                principalColumn: "flight_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Passengers_passenger_id",
                table: "Bookings",
                column: "passenger_id",
                principalTable: "Passengers",
                principalColumn: "passenger_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airlines_airline_id",
                table: "Flights",
                column: "airline_id",
                principalTable: "Airlines",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airplanes_airplane_id",
                table: "Flights",
                column: "airplane_id",
                principalTable: "Airplanes",
                principalColumn: "airplane_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_from",
                table: "Flights",
                column: "from",
                principalTable: "Airports",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flights_Airports_to",
                table: "Flights",
                column: "to",
                principalTable: "Airports",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightSchedules_Airlines_airline_id",
                table: "FlightSchedules",
                column: "airline_id",
                principalTable: "Airlines",
                principalColumn: "airline_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightSchedules_Airports_from",
                table: "FlightSchedules",
                column: "from",
                principalTable: "Airports",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightSchedules_Airports_to",
                table: "FlightSchedules",
                column: "to",
                principalTable: "Airports",
                principalColumn: "airport_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Airlines_Airports_base_airport",
                table: "Airlines");

            migrationBuilder.DropForeignKey(
                name: "FK_Airplanes_Airlines_airline_id",
                table: "Airplanes");

            migrationBuilder.DropForeignKey(
                name: "FK_Airplanes_Airplane_type_type_id",
                table: "Airplanes");

            migrationBuilder.DropForeignKey(
                name: "FK_AirportGeos_Airports_airport_id",
                table: "AirportGeos");

            migrationBuilder.DropForeignKey(
                name: "FK_AirportStaff_Airlines_airline_id",
                table: "AirportStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_AirportStaff_Airports_airport_id",
                table: "AirportStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Flights_flight_id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Passengers_passenger_id",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airlines_airline_id",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airplanes_airplane_id",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_from",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_Flights_Airports_to",
                table: "Flights");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightSchedules_Airlines_airline_id",
                table: "FlightSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightSchedules_Airports_from",
                table: "FlightSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightSchedules_Airports_to",
                table: "FlightSchedules");

            migrationBuilder.DropTable(
                name: "Airplane_type");

            migrationBuilder.DropTable(
                name: "Airport_reachable");

            migrationBuilder.DropTable(
                name: "Passenger_details");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_airline_id",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_from",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_to",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Flights_airline_id",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_airplane_id",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_from",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Flights_to",
                table: "Flights");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_flight_id",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_passenger_id",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Airplanes_airline_id",
                table: "Airplanes");

            migrationBuilder.DropIndex(
                name: "IX_Airplanes_type_id",
                table: "AirPlanes");

            migrationBuilder.DropIndex(
                name: "IX_Airlines_base_airport",
                table: "Airlines");

            migrationBuilder.AlterColumn<int>(
                name: "airport_id",
                table: "AirportGeos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_AirportStaff_Airlines_airline_id",
                table: "AirportStaff",
                column: "airline_id",
                principalTable: "Airlines",
                principalColumn: "airline_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AirportStaff_Airports_airport_id",
                table: "AirportStaff",
                column: "airport_id",
                principalTable: "Airports",
                principalColumn: "airport_id");
        }
    }
}
