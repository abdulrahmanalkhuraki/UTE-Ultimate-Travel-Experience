using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTourPkgToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TourPackages_TourPackageId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Countries_NationalityCountryId",
                table: "TouristGuides");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_PaymentStatuses",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Companion_Relationship",
                table: "Companions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_BookingStatus",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "Bookings");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_PaymentStatuses",
                table: "Payments",
                sql: "[PaymentStatus] IN (0, 1, 2, 3)");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Companion_Relationship",
                table: "Companions",
                sql: "[Relationship] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_BookingStatus",
                table: "Bookings",
                sql: "[Status] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TourPackages_TourPackageId",
                table: "Bookings",
                column: "TourPackageId",
                principalTable: "TourPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristGuides_Countries_NationalityCountryId",
                table: "TouristGuides",
                column: "NationalityCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TourPackages_TourPackageId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Countries_NationalityCountryId",
                table: "TouristGuides");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_PaymentStatuses",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Companion_Relationship",
                table: "Companions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_BookingStatus",
                table: "Bookings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_FlightCabinClass",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "FlightCabinClass",
                table: "Bookings",
                newName: "PackageId");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TourPackageId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FlightType",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CHK_PaymentMethods",
                table: "Payments",
                sql: "[PaymentMethod] IN ('Credit','Bank_Transfer','Digital_Wallet')");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_PaymentStatuses",
                table: "Payments",
                sql: "[PaymentStatus] IN ('Pending','Completed','Failed','Cancelled')");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Companion_Relationship",
                table: "Companions",
                sql: "[Relationship] IN ('Spouse', 'Child', 'Parent', 'Sibling', 'Friend', 'Relative', 'Colleague', 'Guardian', 'Partner', 'Other')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_BookingStatus",
                table: "Bookings",
                sql: "[Status] IN ('Pending', 'Confirmed', 'In_Progress', 'Completed', 'Cancelled', 'No_Show')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_FlightType",
                table: "Bookings",
                sql: "[FlightType] IN ('Economy', 'Premium_Economy', 'Business_Class', 'First_Class')");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TourPackages_TourPackageId",
                table: "Bookings",
                column: "TourPackageId",
                principalTable: "TourPackages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristGuides_Countries_NationalityCountryId",
                table: "TouristGuides",
                column: "NationalityCountryId",
                principalTable: "Countries",
                principalColumn: "Id");
        }
    }
}
