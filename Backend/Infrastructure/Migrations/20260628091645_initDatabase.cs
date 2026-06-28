using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttractionCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnCategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArCategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionCategories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Cities__CountryI__60A75C0F",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttractionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AttractionCategoryId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    OpenAt = table.Column<TimeOnly>(type: "time", nullable: true),
                    ClosedAt = table.Column<TimeOnly>(type: "time", nullable: true),
                    Entry_Fee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attractions_AttractionCategories_AttractionCategoryId",
                        column: x => x.AttractionCategoryId,
                        principalTable: "AttractionCategories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attractions_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProfileImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NationalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NationalIdCard = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PassportNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PassportScan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResidencyCard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalityCountryId = table.Column<int>(type: "int", nullable: false),
                    ResidentialCityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Cities_ResidentialCityId",
                        column: x => x.ResidentialCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Persons_Countries_NationalityCountryId",
                        column: x => x.NationalityCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TouristGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    YearsOfExperiance = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Languages = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LicenseScan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    NatinalityCountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TouristGuides", x => x.Id);
                    table.CheckConstraint("CHK_Positive_YearsOfExperience", "[YearsOfExperiance] BETWEEN 0 AND 70");
                    table.ForeignKey(
                        name: "FK_TouristGuides_Countries_NatinalityCountryId",
                        column: x => x.NatinalityCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TouristGuides_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    BankAccount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Users__RoleId__4F7CD00D",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Relationship = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NationalityCountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companions", x => x.Id);
                    table.CheckConstraint("CHK_Companion_Relationship", "[Relationship] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)");
                    table.ForeignKey(
                        name: "FK_Companions_Countries_NationalityCountryId",
                        column: x => x.NationalityCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Companions_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Companions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "EmailVerification"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EmailVer__3214EC075FAC316A", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailVerifications_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.CheckConstraint("CHK_PaymentStatuses", "[PaymentStatus] IN (0, 1, 2, 3)");
                    table.ForeignKey(
                        name: "FK__payments__UserId__756D6ECB",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

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
                name: "TourCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: true),
                    FoundingDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TourismLicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TourismLicenseImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BankAccount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    About = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TourCompa__UserI__5812160E",
                        column: x => x.UserId,
                        principalTable: "Users",
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

            migrationBuilder.CreateTable(
                name: "Company_TouristGuide",
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
                    table.PrimaryKey("PK_Company_TouristGuide", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyGuides_TourCompanies",
                        column: x => x.CompanyId,
                        principalTable: "TourCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyGuides_TouristGuides",
                        column: x => x.TouristGuideId,
                        principalTable: "TouristGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Favorites__Compa__3A4CA8FD",
                        column: x => x.CompanyId,
                        principalTable: "TourCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Favorites__UserI__3B40CD36",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TourPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MeetingPoint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PricePerPerson = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    EconomyClassPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PremiumClassPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BusinessClassPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RegistrationDeadline = table.Column<DateOnly>(type: "date", nullable: false),
                    ServiceLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PublishCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPackages_Countries",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__TourPacka__Compa__656C112C",
                        column: x => x.CompanyId,
                        principalTable: "TourCompanies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    BookingNumber = table.Column<string>(type: "varchar(20)", nullable: true, computedColumnSql: "CONVERT(varchar(8), [BookingDate], 112) + RIGHT('000000' + CAST([Id] AS varchar(6)), 6)", stored: true),
                    NumberOfAdults = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    NumberOfChildren = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RoomTypePreference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DietaryRequirements = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SpecialRequests = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TourPackageId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FlightCabinClass = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.CheckConstraint("CK_Booking_BookingStatus", "[Status] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9)");
                    table.CheckConstraint("CK_Booking_FlightCabinClass", "[FlightCabinClass] IN (0, 1, 2, 3)");
                    table.ForeignKey(
                        name: "FK_Bookings_TourPackages_TourPackageId",
                        column: x => x.TourPackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_payments",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Bookings__UserId__0F624AF8",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Itineraries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    DayTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DayDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itineraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PackageIt__Packa__6A30C649",
                        column: x => x.PackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PackageAttractions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    AttractionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageAttractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageCities_Attractions",
                        column: x => x.AttractionId,
                        principalTable: "Attractions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PackageCities_TourPackages",
                        column: x => x.PackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RateValue = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Rates__PackageId__3587F3E0",
                        column: x => x.PackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Rates__UserId__3493CFA7",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PackageId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Reviews__Package__1EA48E88",
                        column: x => x.PackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Reviews__UserId__1CBC4616",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TourPackage_TouristGuide",
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
                    table.PrimaryKey("PK_TourPackage_TouristGuide", x => x.Id);
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
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TourPackageId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Wishlists__Attra__2FCF1A8A",
                        column: x => x.TourPackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Wishlists__UserI__2EDAF651",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Companion_Booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanionId = table.Column<int>(type: "int", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companion_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companion_Booking_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Companion_Booking_Companions_CompanionId",
                        column: x => x.CompanionId,
                        principalTable: "Companions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingId = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Notificat__Booki__17F790F9",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__17036CC0",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time(0)", nullable: false),
                    ItineraryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PackageIt__Itine__2645B050",
                        column: x => x.ItineraryId,
                        principalTable: "Itineraries",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AttractionCategories",
                columns: new[] { "CategoryId", "ArCategoryName", "EnCategoryName" },
                values: new object[,]
                {
                    { 1, "متاحف", "Museums" },
                    { 2, "مواقع تاريخية", "Historical Sites" },
                    { 3, "حدائق وطبيعة", "Parks & Nature" },
                    { 4, "مدن ملاهي", "Amusement Parks" },
                    { 5, "شواطئ", "Beaches" },
                    { 6, "مراكز تسوق", "Shopping Malls" },
                    { 7, "حدائق حيوان وأحواض أسماك", "Zoos & Aquariums" },
                    { 8, "مواقع دينية", "Religious Sites" },
                    { 9, "مسارح وعروض", "Theaters & Shows" },
                    { 10, "معارض فنية", "Art Galleries" },
                    { 11, "معالم ونصب تذكارية", "Landmarks & Monuments" },
                    { 12, "قلاع وقصور", "Castles & Palaces" },
                    { 13, "جبال ومسارات مشي", "Mountains & Hiking Trails" },
                    { 14, "حدائق مائية", "Water Parks" },
                    { 15, "ملاعب رياضية", "Sports Arenas" },
                    { 16, "مهرجانات وفعاليات", "Festivals & Events" },
                    { 17, "منتجعات صحية", "Spas & Wellness" },
                    { 18, "أسواق محلية وبازارات", "Local Markets & Bazaars" },
                    { 19, "محميات طبيعية", "Nature Reserves" },
                    { 20, "منصات مشاهدة", "Observation Decks" },
                    { 21, "كهوف", "Caves" },
                    { 22, "منتجعات تزلج", "Ski Resorts" },
                    { 23, "جزر", "Islands" },
                    { 24, "شلالات", "Waterfalls" }
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CountryCode", "CountryName" },
                values: new object[,]
                {
                    { 1, "AF", "Afghanistan" },
                    { 2, "AX", "Åland Islands" },
                    { 3, "AL", "Albania" },
                    { 4, "DZ", "Algeria" },
                    { 5, "AS", "American Samoa" },
                    { 6, "AD", "AndorrA" },
                    { 7, "AO", "Angola" },
                    { 8, "AI", "Anguilla" },
                    { 9, "AQ", "Antarctica" },
                    { 10, "AG", "Antigua and Barbuda" },
                    { 11, "AR", "Argentina" },
                    { 12, "AM", "Armenia" },
                    { 13, "AW", "Aruba" },
                    { 14, "AU", "Australia" },
                    { 15, "AT", "Austria" },
                    { 16, "AZ", "Azerbaijan" },
                    { 17, "BS", "Bahamas" },
                    { 18, "BH", "Bahrain" },
                    { 19, "BD", "Bangladesh" },
                    { 20, "BB", "Barbados" },
                    { 21, "BY", "Belarus" },
                    { 22, "BE", "Belgium" },
                    { 23, "BZ", "Belize" },
                    { 24, "BJ", "Benin" },
                    { 25, "BM", "Bermuda" },
                    { 26, "BT", "Bhutan" },
                    { 27, "BO", "Bolivia" },
                    { 28, "BA", "Bosnia and Herzegovina" },
                    { 29, "BW", "Botswana" },
                    { 30, "BV", "Bouvet Island" },
                    { 31, "BR", "Brazil" },
                    { 32, "IO", "British Indian Ocean Territory" },
                    { 33, "BN", "Brunei Darussalam" },
                    { 34, "BG", "Bulgaria" },
                    { 35, "BF", "Burkina Faso" },
                    { 36, "BI", "Burundi" },
                    { 37, "KH", "Cambodia" },
                    { 38, "CM", "Cameroon" },
                    { 39, "CA", "Canada" },
                    { 40, "CV", "Cape Verde" },
                    { 41, "KY", "Cayman Islands" },
                    { 42, "CF", "Central African Republic" },
                    { 43, "TD", "Chad" },
                    { 44, "CL", "Chile" },
                    { 45, "CN", "China" },
                    { 46, "CX", "Christmas Island" },
                    { 47, "CC", "Cocos (Keeling) Islands" },
                    { 48, "CO", "Colombia" },
                    { 49, "KM", "Comoros" },
                    { 50, "CG", "Congo" },
                    { 51, "CD", "Congo, The Democratic Republic of the" },
                    { 52, "CK", "Cook Islands" },
                    { 53, "CR", "Costa Rica" },
                    { 54, "CI", "Cote D\"Ivoire" },
                    { 55, "HR", "Croatia" },
                    { 56, "CU", "Cuba" },
                    { 57, "CY", "Cyprus" },
                    { 58, "CZ", "Czech Republic" },
                    { 59, "DK", "Denmark" },
                    { 60, "DJ", "Djibouti" },
                    { 61, "DM", "Dominica" },
                    { 62, "DO", "Dominican Republic" },
                    { 63, "EC", "Ecuador" },
                    { 64, "EG", "Egypt" },
                    { 65, "SV", "El Salvador" },
                    { 66, "GQ", "Equatorial Guinea" },
                    { 67, "ER", "Eritrea" },
                    { 68, "EE", "Estonia" },
                    { 69, "ET", "Ethiopia" },
                    { 70, "FK", "Falkland Islands (Malvinas)" },
                    { 71, "FO", "Faroe Islands" },
                    { 72, "FJ", "Fiji" },
                    { 73, "FI", "Finland" },
                    { 74, "FR", "France" },
                    { 75, "GF", "French Guiana" },
                    { 76, "PF", "French Polynesia" },
                    { 77, "TF", "French Southern Territories" },
                    { 78, "GA", "Gabon" },
                    { 79, "GM", "Gambia" },
                    { 80, "GE", "Georgia" },
                    { 81, "DE", "Germany" },
                    { 82, "GH", "Ghana" },
                    { 83, "GI", "Gibraltar" },
                    { 84, "GR", "Greece" },
                    { 85, "GL", "Greenland" },
                    { 86, "GD", "Grenada" },
                    { 87, "GP", "Guadeloupe" },
                    { 88, "GU", "Guam" },
                    { 89, "GT", "Guatemala" },
                    { 90, "GG", "Guernsey" },
                    { 91, "GN", "Guinea" },
                    { 92, "GW", "Guinea-Bissau" },
                    { 93, "GY", "Guyana" },
                    { 94, "HT", "Haiti" },
                    { 95, "HM", "Heard Island and Mcdonald Islands" },
                    { 96, "VA", "Holy See (Vatican City State)" },
                    { 97, "HN", "Honduras" },
                    { 98, "HK", "Hong Kong" },
                    { 99, "HU", "Hungary" },
                    { 100, "IS", "Iceland" },
                    { 101, "IN", "India" },
                    { 102, "ID", "Indonesia" },
                    { 103, "IR", "Iran, Islamic Republic Of" },
                    { 104, "IQ", "Iraq" },
                    { 105, "IE", "Ireland" },
                    { 106, "IM", "Isle of Man" },
                    { 107, "IL", "Israel" },
                    { 108, "IT", "Italy" },
                    { 109, "JM", "Jamaica" },
                    { 110, "JP", "Japan" },
                    { 111, "JE", "Jersey" },
                    { 112, "JO", "Jordan" },
                    { 113, "KZ", "Kazakhstan" },
                    { 114, "KE", "Kenya" },
                    { 115, "KI", "Kiribati" },
                    { 116, "KP", "Korea, Democratic People\"S Republic of" },
                    { 117, "KR", "Korea, Republic of" },
                    { 118, "KW", "Kuwait" },
                    { 119, "KG", "Kyrgyzstan" },
                    { 120, "LA", "Lao People\"S Democratic Republic" },
                    { 121, "LV", "Latvia" },
                    { 122, "LB", "Lebanon" },
                    { 123, "LS", "Lesotho" },
                    { 124, "LR", "Liberia" },
                    { 125, "LY", "Libyan Arab Jamahiriya" },
                    { 126, "LI", "Liechtenstein" },
                    { 127, "LT", "Lithuania" },
                    { 128, "LU", "Luxembourg" },
                    { 129, "MO", "Macao" },
                    { 130, "MK", "Macedonia, The Former Yugoslav Republic of" },
                    { 131, "MG", "Madagascar" },
                    { 132, "MW", "Malawi" },
                    { 133, "MY", "Malaysia" },
                    { 134, "MV", "Maldives" },
                    { 135, "ML", "Mali" },
                    { 136, "MT", "Malta" },
                    { 137, "MH", "Marshall Islands" },
                    { 138, "MQ", "Martinique" },
                    { 139, "MR", "Mauritania" },
                    { 140, "MU", "Mauritius" },
                    { 141, "YT", "Mayotte" },
                    { 142, "MX", "Mexico" },
                    { 143, "FM", "Micronesia, Federated States of" },
                    { 144, "MD", "Moldova, Republic of" },
                    { 145, "MC", "Monaco" },
                    { 146, "MN", "Mongolia" },
                    { 147, "MS", "Montserrat" },
                    { 148, "MA", "Morocco" },
                    { 149, "MZ", "Mozambique" },
                    { 150, "MM", "Myanmar" },
                    { 151, "NA", "Namibia" },
                    { 152, "NR", "Nauru" },
                    { 153, "NP", "Nepal" },
                    { 154, "NL", "Netherlands" },
                    { 155, "AN", "Netherlands Antilles" },
                    { 156, "NC", "New Caledonia" },
                    { 157, "NZ", "New Zealand" },
                    { 158, "NI", "Nicaragua" },
                    { 159, "NE", "Niger" },
                    { 160, "NG", "Nigeria" },
                    { 161, "NU", "Niue" },
                    { 162, "NF", "Norfolk Island" },
                    { 163, "MP", "Northern Mariana Islands" },
                    { 164, "NO", "Norway" },
                    { 165, "OM", "Oman" },
                    { 166, "PK", "Pakistan" },
                    { 167, "PW", "Palau" },
                    { 168, "PS", "Palestinian Territory, Occupied" },
                    { 169, "PA", "Panama" },
                    { 170, "PG", "Papua New Guinea" },
                    { 171, "PY", "Paraguay" },
                    { 172, "PE", "Peru" },
                    { 173, "PH", "Philippines" },
                    { 174, "PN", "Pitcairn" },
                    { 175, "PL", "Poland" },
                    { 176, "PT", "Portugal" },
                    { 177, "PR", "Puerto Rico" },
                    { 178, "QA", "Qatar" },
                    { 179, "RE", "Reunion" },
                    { 180, "RO", "Romania" },
                    { 181, "RU", "Russian Federation" },
                    { 182, "RW", "RWANDA" },
                    { 183, "SH", "Saint Helena" },
                    { 184, "KN", "Saint Kitts and Nevis" },
                    { 185, "LC", "Saint Lucia" },
                    { 186, "PM", "Saint Pierre and Miquelon" },
                    { 187, "VC", "Saint Vincent and the Grenadines" },
                    { 188, "WS", "Samoa" },
                    { 189, "SM", "San Marino" },
                    { 190, "ST", "Sao Tome and Principe" },
                    { 191, "SA", "Saudi Arabia" },
                    { 192, "SN", "Senegal" },
                    { 193, "CS", "Serbia and Montenegro" },
                    { 194, "SC", "Seychelles" },
                    { 195, "SL", "Sierra Leone" },
                    { 196, "SG", "Singapore" },
                    { 197, "SK", "Slovakia" },
                    { 198, "SI", "Slovenia" },
                    { 199, "SB", "Solomon Islands" },
                    { 200, "SO", "Somalia" },
                    { 201, "ZA", "South Africa" },
                    { 202, "GS", "South Georgia and the South Sandwich Islands" },
                    { 203, "ES", "Spain" },
                    { 204, "LK", "Sri Lanka" },
                    { 205, "SD", "Sudan" },
                    { 206, "SR", "Suri" },
                    { 207, "SJ", "Svalbard and Jan Mayen" },
                    { 208, "SZ", "Swaziland" },
                    { 209, "SE", "Sweden" },
                    { 210, "CH", "Switzerland" },
                    { 211, "SY", "Syrian Arab Republic" },
                    { 212, "TW", "Taiwan, Province of China" },
                    { 213, "TJ", "Tajikistan" },
                    { 214, "TZ", "Tanzania, United Republic of" },
                    { 215, "TH", "Thailand" },
                    { 216, "TL", "Timor-Leste" },
                    { 217, "TG", "Togo" },
                    { 218, "TK", "Tokelau" },
                    { 219, "TO", "Tonga" },
                    { 220, "TT", "Trinidad and Tobago" },
                    { 221, "TN", "Tunisia" },
                    { 222, "TR", "Turkey" },
                    { 223, "TM", "Turkmenistan" },
                    { 224, "TC", "Turks and Caicos Islands" },
                    { 225, "TV", "Tuvalu" },
                    { 226, "UG", "Uganda" },
                    { 227, "UA", "Ukraine" },
                    { 228, "AE", "United Arab Emirates" },
                    { 229, "GB", "United Kingdom" },
                    { 230, "US", "United States" },
                    { 231, "UM", "United States Minor Outlying Islands" },
                    { 232, "UY", "Uruguay" },
                    { 233, "UZ", "Uzbekistan" },
                    { 234, "VU", "Vanuatu" },
                    { 235, "VE", "Venezuela" },
                    { 236, "VN", "Viet Nam" },
                    { 237, "VG", "Virgin Islands, British" },
                    { 238, "VI", "Virgin Islands, U.S." },
                    { 239, "WF", "Wallis and Futuna" },
                    { 240, "EH", "Western Sahara" },
                    { 241, "YE", "Yemen" },
                    { 242, "ZM", "Zambia" },
                    { 243, "ZW", "Zimbabwe" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Tourist" },
                    { 3, "TourCompany" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ItineraryId",
                table: "Activities",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_AttractionCategoryId",
                table: "Attractions",
                column: "AttractionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_CityId",
                table: "Attractions",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PaymentId",
                table: "Bookings",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TourPackageId",
                table: "Bookings",
                column: "TourPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Companion_Booking_BookingId",
                table: "Companion_Booking",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Companion_Booking_CompanionId",
                table: "Companion_Booking",
                column: "CompanionId");

            migrationBuilder.CreateIndex(
                name: "IX_Companions_NationalityCountryId",
                table: "Companions",
                column: "NationalityCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Companions_PersonId",
                table: "Companions",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companions_UserId",
                table: "Companions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_TouristGuide_CompanyId_TouristGuideId",
                table: "Company_TouristGuide",
                columns: new[] { "CompanyId", "TouristGuideId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_TouristGuide_TouristGuideId",
                table: "Company_TouristGuide",
                column: "TouristGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_Token",
                table: "DeviceTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_UserId",
                table: "DeviceTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_UserId",
                table: "EmailVerifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerifications_UserId_IsUsed",
                table: "EmailVerifications",
                columns: new[] { "UserId", "IsUsed" });

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_CompanyId",
                table: "Favorites",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraries_PackageId",
                table: "Itineraries",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BookingId",
                table: "Notifications",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageAttractions_AttractionId",
                table: "PackageAttractions",
                column: "AttractionId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageAttractions_PackageId_AttractionId",
                table: "PackageAttractions",
                columns: new[] { "PackageId", "AttractionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_NationalityCountryId",
                table: "Persons",
                column: "NationalityCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ResidentialCityId",
                table: "Persons",
                column: "ResidentialCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_PackageId",
                table: "Rates",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_UserId",
                table: "Rates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PackageId",
                table: "Reviews",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

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
                name: "IX_TourCompanies_UserId",
                table: "TourCompanies",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TouristGuides_NatinalityCountryId",
                table: "TouristGuides",
                column: "NatinalityCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_TouristGuides_PersonId",
                table: "TouristGuides",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourPackage_TouristGuide_PackageId_TouristGuideId",
                table: "TourPackage_TouristGuide",
                columns: new[] { "PackageId", "TouristGuideId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourPackage_TouristGuide_TouristGuideId",
                table: "TourPackage_TouristGuide",
                column: "TouristGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageCabinClasses_PackageId_CabinClass",
                table: "TourPackageCabinClasses",
                columns: new[] { "PackageId", "CabinClass" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageMedias_TourPackageId",
                table: "TourPackageMedias",
                column: "TourPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackages_CompanyId",
                table: "TourPackages",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackages_CountryId",
                table: "TourPackages",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                column: "PersonId",
                unique: true,
                filter: "[PersonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_TourPackageId",
                table: "Wishlists",
                column: "TourPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Companion_Booking");

            migrationBuilder.DropTable(
                name: "Company_TouristGuide");

            migrationBuilder.DropTable(
                name: "DeviceTokens");

            migrationBuilder.DropTable(
                name: "EmailVerifications");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PackageAttractions");

            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SupportReplies");

            migrationBuilder.DropTable(
                name: "TourPackage_TouristGuide");

            migrationBuilder.DropTable(
                name: "TourPackageCabinClasses");

            migrationBuilder.DropTable(
                name: "TourPackageMedias");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Itineraries");

            migrationBuilder.DropTable(
                name: "Companions");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Attractions");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "TouristGuides");

            migrationBuilder.DropTable(
                name: "TourPackages");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "AttractionCategories");

            migrationBuilder.DropTable(
                name: "TourCompanies");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
