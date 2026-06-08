using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFlightTypePropertyToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_BookingType",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BookingType",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "FlightType",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_FlightType",
                table: "Bookings",
                sql: "[FlightType] IN ('Economy', 'Premium_Economy', 'Business_Class', 'First_Class')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_FlightType",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FlightType",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "BookingType",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Standard");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_BookingType",
                table: "Bookings",
                sql: "[BookingType] IN ('Standard', 'Premium', 'VIP')");
        }
    }
}
