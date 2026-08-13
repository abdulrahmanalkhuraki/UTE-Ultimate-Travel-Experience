using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActivityTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTranslations", x => new { x.ActivityId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_ActivityTranslations_Activities",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttractionCategoryTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionCategoryTranslations", x => new { x.CategoryId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_AttractionCategoryTranslations_AttractionCategories",
                        column: x => x.CategoryId,
                        principalTable: "AttractionCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttractionTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AttractionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionTranslations", x => new { x.AttractionId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_AttractionTranslations_Attractions",
                        column: x => x.AttractionId,
                        principalTable: "Attractions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CityTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityTranslations", x => new { x.CityId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_CityTranslations_Cities",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CountryTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryTranslations", x => new { x.CountryId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_CountryTranslations_Countries",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItineraryTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ItineraryId = table.Column<int>(type: "int", nullable: false),
                    DayTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DayDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItineraryTranslations", x => new { x.ItineraryId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_ItineraryTranslations_Itineraries",
                        column: x => x.ItineraryId,
                        principalTable: "Itineraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourCompanyTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    About = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourCompanyTranslations", x => new { x.CompanyId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_TourCompanyTranslations_TourCompanies",
                        column: x => x.CompanyId,
                        principalTable: "TourCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TouristGuideTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TouristGuideId = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristGuideTranslations", x => new { x.TouristGuideId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_TouristGuideTranslations_TouristGuides",
                        column: x => x.TouristGuideId,
                        principalTable: "TouristGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourPackageTranslations",
                columns: table => new
                {
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    PackageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    MeetingPoint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackageTranslations", x => new { x.PackageId, x.LanguageCode });
                    table.ForeignKey(
                        name: "FK_TourPackageTranslations_TourPackages",
                        column: x => x.PackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityTranslations_LanguageCode",
                table: "ActivityTranslations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionCategoryTranslations_LanguageCode",
                table: "AttractionCategoryTranslations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionTranslations_LanguageCode",
                table: "AttractionTranslations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_CityTranslations_LanguageCode",
                table: "CityTranslations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_CountryTranslations_LanguageCode",
                table: "CountryTranslations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_ItineraryTranslations_LanguageCode",
                table: "ItineraryTranslations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_TourCompanyTranslations_LanguageCode",
                table: "TourCompanyTranslations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_TouristGuideTranslations_LanguageCode",
                table: "TouristGuideTranslations",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageTranslations_LanguageCode",
                table: "TourPackageTranslations",
                column: "LanguageCode");

            // Backfill translation tables from the legacy dual-column / single-column
            // fields, so no content is lost when the legacy columns are dropped.
            migrationBuilder.Sql("""
                INSERT INTO [CountryTranslations] ([CountryId], [LanguageCode], [Name])
                SELECT [Id], 'en', [EnCountryName] FROM [Countries];

                INSERT INTO [CountryTranslations] ([CountryId], [LanguageCode], [Name])
                SELECT [Id], 'ar', [ArCountryName] FROM [Countries]
                WHERE [ArCountryName] IS NOT NULL AND LTRIM(RTRIM([ArCountryName])) <> '';
                """);

            migrationBuilder.Sql("""
                INSERT INTO [CityTranslations] ([CityId], [LanguageCode], [Name])
                SELECT [Id], 'en', [EnCityName] FROM [Cities];

                INSERT INTO [CityTranslations] ([CityId], [LanguageCode], [Name])
                SELECT [Id], 'ar', [ArCityName] FROM [Cities]
                WHERE [ArCityName] IS NOT NULL AND LTRIM(RTRIM([ArCityName])) <> '';
                """);

            migrationBuilder.Sql("""
                INSERT INTO [AttractionTranslations] ([AttractionId], [LanguageCode], [Name], [Description])
                SELECT [Id], 'en', [EnAttractionName], [Description] FROM [Attractions];

                INSERT INTO [AttractionTranslations] ([AttractionId], [LanguageCode], [Name], [Description])
                SELECT [Id], 'ar', [ArAttractionName], NULL FROM [Attractions]
                WHERE [ArAttractionName] IS NOT NULL AND LTRIM(RTRIM([ArAttractionName])) <> '';
                """);

            migrationBuilder.Sql("""
                INSERT INTO [AttractionCategoryTranslations] ([CategoryId], [LanguageCode], [Name])
                SELECT [CategoryId], 'en', [EnCategoryName] FROM [AttractionCategories];

                INSERT INTO [AttractionCategoryTranslations] ([CategoryId], [LanguageCode], [Name])
                SELECT [CategoryId], 'ar', [ArCategoryName] FROM [AttractionCategories]
                WHERE [ArCategoryName] IS NOT NULL AND LTRIM(RTRIM([ArCategoryName])) <> '';
                """);

            migrationBuilder.Sql("""
                INSERT INTO [TourPackageTranslations] ([PackageId], [LanguageCode], [PackageName], [Description], [MeetingPoint])
                SELECT [Id], 'en', [PackageName], COALESCE([Description], N''), [MeetingPoint] FROM [TourPackages];
                """);

            migrationBuilder.Sql("""
                INSERT INTO [ItineraryTranslations] ([ItineraryId], [LanguageCode], [DayTitle], [DayDescription])
                SELECT [Id], 'en', [DayTitle], [DayDescription] FROM [Itineraries];
                """);

            migrationBuilder.Sql("""
                INSERT INTO [ActivityTranslations] ([ActivityId], [LanguageCode], [Title], [Description])
                SELECT [Id], 'en', [Title], [Description] FROM [Activities];
                """);

            migrationBuilder.Sql("""
                INSERT INTO [TourCompanyTranslations] ([CompanyId], [LanguageCode], [Description], [About])
                SELECT [Id], 'en', [Description], [About] FROM [TourCompanies];
                """);

            migrationBuilder.Sql("""
                INSERT INTO [TouristGuideTranslations] ([TouristGuideId], [LanguageCode], [Bio])
                SELECT [Id], 'en', [Bio] FROM [TouristGuides];
                """);

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "MeetingPoint",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "PackageName",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "TouristGuides");

            migrationBuilder.DropColumn(
                name: "About",
                table: "TourCompanies");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TourCompanies");

            migrationBuilder.DropColumn(
                name: "DayDescription",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "DayTitle",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "ArCountryName",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "EnCountryName",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "ArCityName",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "EnCityName",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ArAttractionName",
                table: "Attractions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Attractions");

            migrationBuilder.DropColumn(
                name: "EnAttractionName",
                table: "Attractions");

            migrationBuilder.DropColumn(
                name: "ArCategoryName",
                table: "AttractionCategories");

            migrationBuilder.DropColumn(
                name: "EnCategoryName",
                table: "AttractionCategories");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Activities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityTranslations");

            migrationBuilder.DropTable(
                name: "AttractionCategoryTranslations");

            migrationBuilder.DropTable(
                name: "AttractionTranslations");

            migrationBuilder.DropTable(
                name: "CityTranslations");

            migrationBuilder.DropTable(
                name: "CountryTranslations");

            migrationBuilder.DropTable(
                name: "ItineraryTranslations");

            migrationBuilder.DropTable(
                name: "TourCompanyTranslations");

            migrationBuilder.DropTable(
                name: "TouristGuideTranslations");

            migrationBuilder.DropTable(
                name: "TourPackageTranslations");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TourPackages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MeetingPoint",
                table: "TourPackages",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PackageName",
                table: "TourPackages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "TouristGuides",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "About",
                table: "TourCompanies",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TourCompanies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DayDescription",
                table: "Itineraries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DayTitle",
                table: "Itineraries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArCountryName",
                table: "Countries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnCountryName",
                table: "Countries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArCityName",
                table: "Cities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnCityName",
                table: "Cities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArAttractionName",
                table: "Attractions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Attractions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnAttractionName",
                table: "Attractions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArCategoryName",
                table: "AttractionCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EnCategoryName",
                table: "AttractionCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Activities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Activities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
