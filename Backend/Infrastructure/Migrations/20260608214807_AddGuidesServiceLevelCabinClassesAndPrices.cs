using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidesServiceLevelCabinClassesAndPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Cities_CityId",
                table: "TouristGuides");

            migrationBuilder.DropIndex(
                name: "IX_TouristGuides_CityId",
                table: "TouristGuides");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "TouristGuides");

            migrationBuilder.AddColumn<decimal>(
                name: "BusinessClassPrice",
                table: "TourPackages",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EconomyClassPrice",
                table: "TourPackages",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PremiumClassPrice",
                table: "TourPackages",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ResidentialCountryId",
                table: "TouristGuides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessClassPrice",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "EconomyClassPrice",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "PremiumClassPrice",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "ResidentialCountryId",
                table: "TouristGuides");

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "TouristGuides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TouristGuides_CityId",
                table: "TouristGuides",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristGuides_Cities_CityId",
                table: "TouristGuides",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");
        }
    }
}
