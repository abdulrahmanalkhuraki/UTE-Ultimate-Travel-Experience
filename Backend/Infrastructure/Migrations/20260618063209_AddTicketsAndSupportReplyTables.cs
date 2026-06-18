using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketsAndSupportReplyTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageCities_Cities",
                table: "PackageCities");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Countries_ResidentialCountryId",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK__Reviews__Attract__1DB06A4F",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK__Reviews__Package__1EA48E88",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK__TourCompa__UserI__5812160E",
                table: "TourCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK__Wishlists__Attra__2FCF1A8A",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK__Wishlists__UserI__2EDAF651",
                table: "Wishlists");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_TourCompanies_UserId",
                table: "TourCompanies");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_AttractionId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MainImageUrl",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "CurrentLocation",
                table: "TouristGuides");

            migrationBuilder.DropColumn(
                name: "AttractionId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "AttractionId",
                table: "Wishlists",
                newName: "TourPackageId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlists_AttractionId",
                table: "Wishlists",
                newName: "IX_Wishlists_TourPackageId");

            migrationBuilder.RenameColumn(
                name: "CityId",
                table: "PackageCities",
                newName: "AttractionId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageCities_PackageId_CityId",
                table: "PackageCities",
                newName: "IX_PackageCities_PackageId_AttractionId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageCities_CityId",
                table: "PackageCities",
                newName: "IX_PackageCities_AttractionId");

            migrationBuilder.RenameColumn("ResidentialCountryId", "Persons", "ResidentialCityId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Users",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TourPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "PackageId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "OpenAt",
                table: "Attractions",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "ClosedAt",
                table: "Attractions",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AddColumn<string>(
                name: "BookingNumber",
                table: "Bookings",
                type: "varchar(20)",
                nullable: true,
                computedColumnSql: "CONVERT(varchar(8), [BookingDate], 112) + RIGHT('000000' + CAST([Id] AS varchar(6)), 6)",
                stored: true);

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TourPackageMedias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourPackageId = table.Column<int>(type: "int", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MediaType = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackageMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPackageMedia_TourPackages",
                        column: x => x.TourPackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportReplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    ReplyContent = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportReplies_Admins",
                        column: x => x.AdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportReplies_Tickets",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourCompanies_UserId",
                table: "TourCompanies",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportReplies_AdminId",
                table: "SupportReplies",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportReplies_TicketId",
                table: "SupportReplies",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageMedias_TourPackageId",
                table: "TourPackageMedias",
                column: "TourPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageCities_Attractions",
                table: "PackageCities",
                column: "AttractionId",
                principalTable: "Attractions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Cities_ResidentialCityId",
                table: "Persons",
                column: "ResidentialCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Reviews__Package__1EA48E88",
                table: "Reviews",
                column: "PackageId",
                principalTable: "TourPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__TourCompa__UserI__5812160E",
                table: "TourCompanies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlists__Attra__2FCF1A8A",
                table: "Wishlists",
                column: "TourPackageId",
                principalTable: "TourPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlists__UserI__2EDAF651",
                table: "Wishlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameTable(name: "PackageCities", newName: "TourPackage_Attraction");
            migrationBuilder.RenameTable(name: "TourPackageGuides", newName: "TourPackage_TouristGuide");
            migrationBuilder.RenameTable(name: "CompanyGuides", newName: "Company_TouristGuide");
            migrationBuilder.RenameTable(name: "CompanionBookings", newName: "Companion_Booking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageCities_Attractions",
                table: "PackageCities");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Cities_ResidentialCityId",
                table: "Persons");

            migrationBuilder.DropForeignKey(
                name: "FK__Reviews__Package__1EA48E88",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK__TourCompa__UserI__5812160E",
                table: "TourCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK__Wishlists__Attra__2FCF1A8A",
                table: "Wishlists");

            migrationBuilder.DropForeignKey(
                name: "FK__Wishlists__UserI__2EDAF651",
                table: "Wishlists");

            migrationBuilder.DropTable(
                name: "SupportReplies");

            migrationBuilder.DropTable(
                name: "TourPackageMedias");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_TourCompanies_UserId",
                table: "TourCompanies");

            migrationBuilder.DropColumn(
                name: "BookingNumber",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TourPackages");

            migrationBuilder.RenameColumn(
                name: "TourPackageId",
                table: "Wishlists",
                newName: "AttractionId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlists_TourPackageId",
                table: "Wishlists",
                newName: "IX_Wishlists_AttractionId");

            migrationBuilder.RenameColumn(
                name: "AttractionId",
                table: "PackageCities",
                newName: "CityId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageCities_PackageId_AttractionId",
                table: "PackageCities",
                newName: "IX_PackageCities_PackageId_CityId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageCities_AttractionId",
                table: "PackageCities",
                newName: "IX_PackageCities_CityId");

            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainImageUrl",
                table: "TourPackages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentLocation",
                table: "TouristGuides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PackageId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "AttractionId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "OpenAt",
                table: "Attractions",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "ClosedAt",
                table: "Attractions",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttractionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Images__Attracti__09A971A2",
                        column: x => x.AttractionId,
                        principalTable: "Attractions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourCompanies_UserId",
                table: "TourCompanies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AttractionId",
                table: "Reviews",
                column: "AttractionId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_AttractionId",
                table: "Images",
                column: "AttractionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageCities_Cities",
                table: "PackageCities",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Countries_ResidentialCityId",
                table: "Persons",
                column: "ResidentialCityId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Reviews__Attract__1DB06A4F",
                table: "Reviews",
                column: "AttractionId",
                principalTable: "Attractions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Reviews__Package__1EA48E88",
                table: "Reviews",
                column: "PackageId",
                principalTable: "TourPackages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__TourCompa__UserI__5812160E",
                table: "TourCompanies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlists__Attra__2FCF1A8A",
                table: "Wishlists",
                column: "AttractionId",
                principalTable: "Attractions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlists__UserI__2EDAF651",
                table: "Wishlists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
