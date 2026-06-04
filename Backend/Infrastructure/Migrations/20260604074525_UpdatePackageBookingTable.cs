using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackageBookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfPeople",
                table: "Bookings",
                newName: "NumberOfAdults");

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation",
                table: "TourPackages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DietaryRequirements",
                table: "PackageBookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomTypePreference",
                table: "PackageBookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialRequests",
                table: "PackageBookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfChildren",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupLocation",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "DietaryRequirements",
                table: "PackageBookings");

            migrationBuilder.DropColumn(
                name: "RoomTypePreference",
                table: "PackageBookings");

            migrationBuilder.DropColumn(
                name: "SpecialRequests",
                table: "PackageBookings");

            migrationBuilder.DropColumn(
                name: "NumberOfChildren",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "NumberOfAdults",
                table: "Bookings",
                newName: "NumberOfPeople");
        }
    }
}
