using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PublishCount",
                table: "TourPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAtUtc",
                table: "TourPackages",
                type: "datetime",
                nullable: true);

            // Backfill: treat already-published programs as published once, since they were created.
            migrationBuilder.Sql(
                "UPDATE [TourPackages] SET [PublishCount] = 1, [PublishedAtUtc] = [CreatedAtUtc] WHERE [IsPublished] = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishCount",
                table: "TourPackages");

            migrationBuilder.DropColumn(
                name: "PublishedAtUtc",
                table: "TourPackages");
        }
    }
}
