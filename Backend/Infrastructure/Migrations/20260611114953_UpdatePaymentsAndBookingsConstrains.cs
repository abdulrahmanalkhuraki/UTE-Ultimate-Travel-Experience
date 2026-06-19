using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentsAndBookingsConstrains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_payments",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK__Bookings__UserId__0F624AF8",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Countries_NationalityCountryId",
                table: "TouristGuides");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Companion_Relationship",
                table: "Companions");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PaymentId",
                table: "Bookings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_BookingStatus",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Payments");



            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BookingDate",
                table: "Bookings",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddColumn<int>(
                name: "FlightCabinClass",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "Bookings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Companion_Relationship",
                table: "Companions",
                sql: "[Relationship] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PaymentId",
                table: "Bookings",
                column: "PaymentId",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_BookingStatus",
                table: "Bookings",
                sql: "[Status] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_FlightCabinClass",
                table: "Bookings",
                sql: "[FlightCabinClass] IN (0, 1, 2, 3)");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_payments",
                table: "Bookings",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Bookings__UserId__0F624AF8",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
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
                name: "FK_Bookings_payments",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK__Bookings__UserId__0F624AF8",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Countries_NationalityCountryId",
                table: "TouristGuides");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_PaymentStatuses",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_Companion_Relationship",
                table: "Companions");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PaymentId",
                table: "Bookings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_BookingStatus",
                table: "Bookings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_FlightCabinClass",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FlightCabinClass",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "Bookings");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "PaymentDate",
                table: "Payments",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Payment_Method",
                table: "Payments",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BookingDate",
                table: "Bookings",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<string>(
                name: "BookingType",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Standard");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_Companion_Relationship",
                table: "Companions",
                sql: "[Relationship] IN ('Spouse', 'Child', 'Parent', 'Sibling', 'Friend', 'Relative', 'Colleague', 'Guardian', 'Partner', 'Other')");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PaymentId",
                table: "Bookings",
                column: "PaymentId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_BookingStatus",
                table: "Bookings",
                sql: "[Status] IN ('Pending', 'Confirmed', 'In_Progress', 'Completed', 'Cancelled', 'No_Show')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_BookingType",
                table: "Bookings",
                sql: "[BookingType] IN ('Standard', 'Premium', 'VIP')");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_payments",
                table: "Bookings",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Bookings__UserId__0F624AF8",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
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
