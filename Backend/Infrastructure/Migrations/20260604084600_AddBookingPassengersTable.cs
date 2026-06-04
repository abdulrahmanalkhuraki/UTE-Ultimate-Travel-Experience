using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingPassengersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "TourPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BookingPassenger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fullname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    IdentityType = table.Column<int>(type: "int", nullable: true),
                    IdentityDocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NatinalityCountryID = table.Column<int>(type: "int", nullable: false),
                    BookingID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPassenger", x => x.Id);
                    table.CheckConstraint("CK_BookingPassengers_IdentityType", "[IdentityType] IN ('NationalID','Passport')");
                    table.CheckConstraint("CK_Valid_Age", "[Age] Between 1 and 100");
                    table.ForeignKey(
                        name: "FK_BookingPassenger_Bookings_BookingID",
                        column: x => x.BookingID,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingPassenger_Countries_NatinalityCountryID",
                        column: x => x.NatinalityCountryID,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourPackages_CountryId",
                table: "TourPackages",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassenger_BookingID",
                table: "BookingPassenger",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassenger_NatinalityCountryID",
                table: "BookingPassenger",
                column: "NatinalityCountryID");

            migrationBuilder.AddForeignKey(
                name: "FK_TourPackages_Countries_CountryId",
                table: "TourPackages",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourPackages_Countries_CountryId",
                table: "TourPackages");

            migrationBuilder.DropTable(
                name: "BookingPassenger");

            migrationBuilder.DropIndex(
                name: "IX_TourPackages_CountryId",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "TourPackages");
        }
    }
}
