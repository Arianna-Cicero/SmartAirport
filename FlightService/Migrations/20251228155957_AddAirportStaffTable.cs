using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FlightService.Migrations
{
    /// <inheritdoc />
    public partial class AddAirportStaffTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirportStaff",
                columns: table => new
                {
                    staff_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    airport_id = table.Column<int>(type: "integer", nullable: true),
                    airline_id = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    failed_login_attempts = table.Column<int>(type: "integer", nullable: false),
                    locked_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_reset_token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password_reset_expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirportStaff", x => x.staff_id);
                    table.ForeignKey(
                        name: "FK_AirportStaff_Airlines_airline_id",
                        column: x => x.airline_id,
                        principalTable: "Airlines",
                        principalColumn: "airline_id");
                    table.ForeignKey(
                        name: "FK_AirportStaff_Airports_airport_id",
                        column: x => x.airport_id,
                        principalTable: "Airports",
                        principalColumn: "airport_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirportStaff_airline_id",
                table: "AirportStaff",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "IX_AirportStaff_airport_id",
                table: "AirportStaff",
                column: "airport_id");

            migrationBuilder.CreateIndex(
                name: "IX_AirportStaff_email",
                table: "AirportStaff",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AirportStaff_username",
                table: "AirportStaff",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirportStaff");
        }
    }
}
