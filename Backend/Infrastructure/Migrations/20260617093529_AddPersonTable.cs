using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_Companions_Countries_ResidentialCountryId",
                table: "Companions");

            migrationBuilder.DropForeignKey(
                name: "FK_Companions_Users_UserId",
                table: "Companions");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageItineraryAttractions_Attractions_AttractionId",
                table: "PackageItineraryAttractions");

            migrationBuilder.DropTable(
                name: "AttractionActivities");

            migrationBuilder.DropTable(
                name: "TourPackageFlights");

            migrationBuilder.DropTable(
                name: "TourPackageHotels");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropIndex(
            name: "IX_PackageItineraryAttractions_AttractionId",
            table: "PackageItineraryAttractions");

            migrationBuilder.Sql(@"IF COL_LENGTH('PackageItineraryAttractions','AttractionId') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('PackageItineraryAttractions') AND c.name = 'AttractionId'; IF @df IS NOT NULL EXEC('ALTER TABLE [PackageItineraryAttractions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [PackageItineraryAttractions] DROP COLUMN [AttractionId]; END");

            migrationBuilder.RenameTable(
                name: "PackageItineraryAttractions",
                newName: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Companions_ResidentialCountryId",
                table: "Companions");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','Date_Of_Birth') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'Date_Of_Birth'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [Date_Of_Birth]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','FirstName') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'FirstName'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [FirstName]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','Gender') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'Gender'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [Gender]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','IsProfileCompleted') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'IsProfileCompleted'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [IsProfileCompleted]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','LastName') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'LastName'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [LastName]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','NationalIdImage') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'NationalIdImage'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [NationalIdImage]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','NationalNumber') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'NationalNumber'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [NationalNumber]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','PassportNumber') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'PassportNumber'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [PassportNumber]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','PassportImage') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'PassportImage'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [PassportImage]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','Phone') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'Phone'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [Phone]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','Image') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'Image'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [Image]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','PlaceOfResidence') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'PlaceOfResidence'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [PlaceOfResidence]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','CreatedAtUtc') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'CreatedAtUtc'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [CreatedAtUtc]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','DateOfBirth') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'DateOfBirth'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [DateOfBirth]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','Firstname') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'Firstname'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [Firstname]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','Gender') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'Gender'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [Gender]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','Lastname') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'Lastname'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [Lastname]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','IdCard') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'IdCard'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [IdCard]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','NationalNumber') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'NationalNumber'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [NationalNumber]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','PassportNumber') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'PassportNumber'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [PassportNumber]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','PassportScan') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'PassportScan'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [PassportScan]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','Phone') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'Phone'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [Phone]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','PlaceOfResidence') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'PlaceOfResidence'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [PlaceOfResidence]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','UpdatedAtUtc') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'UpdatedAtUtc'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [UpdatedAtUtc]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Companions','CreatedAtUtc') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Companions') AND c.name = 'CreatedAtUtc'; IF @df IS NOT NULL EXEC('ALTER TABLE [Companions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Companions] DROP COLUMN [CreatedAtUtc]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Companions','DateOfBirth') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Companions') AND c.name = 'DateOfBirth'; IF @df IS NOT NULL EXEC('ALTER TABLE [Companions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Companions] DROP COLUMN [DateOfBirth]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Companions','Firstname') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Companions') AND c.name = 'Firstname'; IF @df IS NOT NULL EXEC('ALTER TABLE [Companions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Companions] DROP COLUMN [Firstname]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Companions','Gender') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Companions') AND c.name = 'Gender'; IF @df IS NOT NULL EXEC('ALTER TABLE [Companions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Companions] DROP COLUMN [Gender]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Companions','Lastname') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Companions') AND c.name = 'Lastname'; IF @df IS NOT NULL EXEC('ALTER TABLE [Companions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Companions] DROP COLUMN [Lastname]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Companions','IdCard') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Companions') AND c.name = 'IdCard'; IF @df IS NOT NULL EXEC('ALTER TABLE [Companions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Companions] DROP COLUMN [IdCard]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Companions','Phone') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Companions') AND c.name = 'Phone'; IF @df IS NOT NULL EXEC('ALTER TABLE [Companions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Companions] DROP COLUMN [Phone]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('Companions','UpdatedAtUtc') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Companions') AND c.name = 'UpdatedAtUtc'; IF @df IS NOT NULL EXEC('ALTER TABLE [Companions] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Companions] DROP COLUMN [UpdatedAtUtc]; END");

            migrationBuilder.RenameColumn(
                name: "ResidentialCityId",
                table: "Companions",
                newName: "PersonId");

            migrationBuilder.RenameColumn(
                name: "PassportScan",
                table: "Companions",
                newName: "ResidencyCard");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "TouristGuides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "Bookings",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

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
                    ResidentialCountryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Countries_ResidentialCountryId",
                        column: x => x.ResidentialCountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PersonId",
                table: "Users",
                column: "PersonId",
                unique: true,
                filter: "[PersonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TouristGuides_PersonId",
                table: "TouristGuides",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companions_PersonId",
                table: "Companions",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ResidentialCountryId",
                table: "Persons",
                column: "ResidentialCityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companions_Persons_PersonId",
                table: "Companions",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Companions_Users_UserId",
                table: "Companions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TouristGuides_Persons_PersonId",
                table: "TouristGuides",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_PersonId",
                table: "Users",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companions_Persons_PersonId",
                table: "Companions");

            migrationBuilder.DropForeignKey(
                name: "FK_Companions_Users_UserId",
                table: "Companions");

            migrationBuilder.DropForeignKey(
                name: "FK_TouristGuides_Persons_PersonId",
                table: "TouristGuides");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_PersonId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Users_PersonId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TouristGuides_PersonId",
                table: "TouristGuides");

            migrationBuilder.DropIndex(
                name: "IX_Companions_PersonId",
                table: "Companions");

            migrationBuilder.Sql(@"IF COL_LENGTH('Users','PersonId') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('Users') AND c.name = 'PersonId'; IF @df IS NOT NULL EXEC('ALTER TABLE [Users] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [Users] DROP COLUMN [PersonId]; END");

            migrationBuilder.Sql(@"IF COL_LENGTH('TouristGuides','PersonId') IS NOT NULL BEGIN DECLARE @df sysname; SELECT @df = d.name FROM sys.default_constraints d JOIN sys.columns c ON c.default_object_id = d.object_id WHERE c.object_id = OBJECT_ID('TouristGuides') AND c.name = 'PersonId'; IF @df IS NOT NULL EXEC('ALTER TABLE [TouristGuides] DROP CONSTRAINT [' + @df + ']'); ALTER TABLE [TouristGuides] DROP COLUMN [PersonId]; END");

            migrationBuilder.RenameColumn(
                name: "ResidencyCard",
                table: "Companions",
                newName: "PassportScan");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "Companions",
                newName: "ResidentialCityId");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Cities",
                newName: "ProfileImage");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date_Of_Birth",
                table: "Users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProfileCompleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalIdCard",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalNumber",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportScan",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResidentialCityId",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "TouristGuides",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "TouristGuides",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "TouristGuides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Gender",
                table: "TouristGuides",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                table: "TouristGuides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdCard",
                table: "TouristGuides",
                type: "nvarchar(500)",
                maxLength: 500,
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
                name: "PassportScan",
                table: "TouristGuides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "TouristGuides",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "TouristGuides",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResidentialCityId",
                table: "TouristGuides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "TouristGuides",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Companions",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                table: "Companions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Firstname",
                table: "Companions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Gender",
                table: "Companions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                table: "Companions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdCard",
                table: "Companions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Companions",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Companions",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCost",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttractionId",
                table: "Activities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Activities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Activities",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AttractionActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    AttractionId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttractionActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Attractio__Activ__2180FB33",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Attractio__Attra__22751F6C",
                        column: x => x.AttractionId,
                        principalTable: "Attractions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArrivalCityId = table.Column<int>(type: "int", nullable: false),
                    DepartureCityId = table.Column<int>(type: "int", nullable: false),
                    Airline = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Arrival = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Departure = table.Column<DateTime>(type: "datetime", nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flights_Cities_ArrivalCityId",
                        column: x => x.ArrivalCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Flights_Cities_DepartureCityId",
                        column: x => x.DepartureCityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HotelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StarRating = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TourPackageFlights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    TourPackageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackageFlights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourPackageFlights_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TourPackageFlights_TourPackages_TourPackageId",
                        column: x => x.TourPackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TourPackageHotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    TourPackageId = table.Column<int>(type: "int", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOut = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourPackageHotels", x => x.Id);
                    table.CheckConstraint("CHK_CheckIn_CheckOut", "[CheckOut] > [CheckIn]");
                    table.CheckConstraint("CHK_Future_CheckIn", "[CheckIn] > GETDATE()");
                    table.ForeignKey(
                        name: "FK_TourPackageHotels_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TourPackageHotels_TourPackages_TourPackageId",
                        column: x => x.TourPackageId,
                        principalTable: "TourPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companions_ResidentialCountryId",
                table: "Companions",
                column: "ResidentialCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_AttractionId",
                table: "Activities",
                column: "AttractionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionActivities_ActivityId",
                table: "AttractionActivities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_AttractionActivities_AttractionId",
                table: "AttractionActivities",
                column: "AttractionId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_ArrivalCityId",
                table: "Flights",
                column: "ArrivalCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DepartureCityId",
                table: "Flights",
                column: "DepartureCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CityId",
                table: "Hotels",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageFlights_FlightId",
                table: "TourPackageFlights",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageFlights_TourPackageId",
                table: "TourPackageFlights",
                column: "TourPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageHotels_HotelId",
                table: "TourPackageHotels",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_TourPackageHotels_TourPackageId",
                table: "TourPackageHotels",
                column: "TourPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Attractions_AttractionId",
                table: "Activities",
                column: "AttractionId",
                principalTable: "Attractions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Companions_Countries_ResidentialCountryId",
                table: "Companions",
                column: "ResidentialCityId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companions_Users_UserId",
                table: "Companions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
