using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidesServiceLevelAndCabinClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Cities_CityId",
                table: "TouristGuides");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Countries_NatinalityCountryId",
                table: "TouristGuides");

            migrationBuilder.DropForeignKey(
                name: "FK_TourPackages_TouristGuides_TouristGuideId",
                table: "TourPackages");

            migrationBuilder.DropIndex(
                name: "IX_TourPackages_TouristGuideId",
                table: "TourPackages");

            migrationBuilder.DropIndex(
                name: "IX_TouristGuides_NatinalityCountryId",
                table: "TouristGuides");

            migrationBuilder.DropColumn(
                name: "TourGuide",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "TouristGuideId",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "NatinalityCountryId",
                table: "TouristGuides");

            migrationBuilder.AddColumn<int>(
                name: "ServiceLevel",
                table: "TourPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PassportScan",
                table: "TouristGuides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseScan",
                table: "TouristGuides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Languages",
                table: "TouristGuides",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "TouristGuides",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IdCard",
                table: "TouristGuides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "TouristGuides",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                table: "TouristGuides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalNumber",
                table: "TouristGuides",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "TouristGuides",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "TouristGuides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    TouristGuideId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyGuides_TourCompanies",
                        column: x => x.CompanyId,
                        principalTable: "TourCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyGuides_TouristGuides",
                        column: x => x.TouristGuideId,
                        principalTable: "TouristGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourPackageCabinClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    CabinClass = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackageCabinClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPackageCabinClasses_TourPackages",
                        column: x => x.PackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourPackageGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    TouristGuideId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackageGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPackageGuides_TourPackages",
                        column: x => x.PackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TourPackageGuides_TouristGuides",
                        column: x => x.TouristGuideId,
                        principalTable: "TouristGuides",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TouristGuides_NationalityCountryId",
                table: "TouristGuides",
                column: "NationalityCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyGuides_CompanyId_TouristGuideId",
                table: "CompanyGuides",
                columns: new[] { "CompanyId", "TouristGuideId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyGuides_TouristGuideId",
                table: "CompanyGuides",
                column: "TouristGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageCabinClasses_PackageId_CabinClass",
                table: "TourPackageCabinClasses",
                columns: new[] { "PackageId", "CabinClass" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageGuides_PackageId_TouristGuideId",
                table: "TourPackageGuides",
                columns: new[] { "PackageId", "TouristGuideId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageGuides_TouristGuideId",
                table: "TourPackageGuides",
                column: "TouristGuideId");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristGuides_Cities_CityId",
                table: "TouristGuides",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristGuides_Countries_NationalityCountryId",
                table: "TouristGuides",
                column: "NationalityCountryId",
                principalTable: "Countries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Cities_CityId",
                table: "TouristGuides");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Countries_NationalityCountryId",
                table: "TouristGuides");

            migrationBuilder.DropTable(
                name: "CompanyGuides");

            migrationBuilder.DropTable(
                name: "TourPackageCabinClasses");

            migrationBuilder.DropTable(
                name: "TourPackageGuides");

            migrationBuilder.DropIndex(
                name: "IX_TouristGuides_NationalityCountryId",
                table: "TouristGuides");

            migrationBuilder.DropColumn(
                name: "ServiceLevel",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                table: "TouristGuides");

            migrationBuilder.DropColumn(
                name: "NationalNumber",
                table: "TouristGuides");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "TouristGuides");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "TouristGuides");

            migrationBuilder.AddColumn<string>(
                name: "TourGuide",
                table: "TourPackages",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TouristGuideId",
                table: "TourPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PassportScan",
                table: "TouristGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseScan",
                table: "TouristGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Languages",
                table: "TouristGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "TouristGuides",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdCard",
                table: "TouristGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "TouristGuides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<int>(
                name: "NatinalityCountryId",
                table: "TouristGuides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TourPackages_TouristGuideId",
                table: "TourPackages",
                column: "TouristGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TouristGuides_NatinalityCountryId",
                table: "TouristGuides",
                column: "NatinalityCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_TouristGuides_Cities_CityId",
                table: "TouristGuides",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristGuides_Countries_NatinalityCountryId",
                table: "TouristGuides",
                column: "NatinalityCountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TourPackages_TouristGuides_TouristGuideId",
                table: "TourPackages",
                column: "TouristGuideId",
                principalTable: "TouristGuides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
