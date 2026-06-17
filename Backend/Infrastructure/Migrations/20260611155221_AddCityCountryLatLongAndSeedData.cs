using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCityCountryLatLongAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Countries",
                type: "decimal(10,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Countries",
                type: "decimal(10,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Cities",
                type: "decimal(10,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Cities",
                type: "decimal(10,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CountryCode", "CountryName", "Flag", "Latitude", "Longitude" },
                values: new object[,]
                {
                    { 2, "JO", "Jordan", null, 31.945400m, 35.928400m },
                    { 3, "SY", "Syria", null, 33.513800m, 36.276500m },
                    { 4, "LB", "Lebanon", null, 33.893800m, 35.501800m },
                    { 5, "EG", "Egypt", null, 30.044400m, 31.235700m },
                    { 6, "AE", "United Arab Emirates", null, 24.453900m, 54.377300m },
                    { 7, "TR", "Turkey", null, 39.933400m, 32.859700m },
                    { 8, "SA", "Saudi Arabia", null, 24.713600m, 46.675300m }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CityName", "CountryId", "Description", "ProfileImage", "Latitude", "Longitude" },
                values: new object[,]
                {
                    { 2, "Amman", 1, "The capital of Jordan.", null, 31.945400m, 35.928400m },
                    { 3, "Aqaba", 1, "Red Sea coastal city and diving hub.", null, 29.526700m, 35.007800m },
                    { 4, "Petra", 1, "Ancient rose-red city, a wonder of the world.", null, 30.328500m, 35.444400m },
                    { 5, "Irbid", 1, "Northern university city.", null, 32.555600m, 35.850000m },
                    { 6, "Damascus", 2, "One of the oldest continuously inhabited cities.", null, 33.513800m, 36.276500m },
                    { 7, "Aleppo", 2, "Historic city famous for its citadel and souks.", null, 36.202100m, 37.134300m },
                    { 8, "Homs", 2, "Central Syrian city.", null, 34.732400m, 36.713700m },
                    { 9, "Latakia", 2, "Main Mediterranean port city.", null, 35.519600m, 35.791500m },
                    { 10, "Beirut", 3, "The capital and cultural heart of Lebanon.", null, 33.893800m, 35.501800m },
                    { 11, "Tripoli", 3, "Northern city rich in Mamluk architecture.", null, 34.436700m, 35.849700m },
                    { 12, "Byblos", 3, "Ancient port, among the oldest cities in the world.", null, 34.123200m, 35.651000m },
                    { 13, "Cairo", 4, "The capital, home to the Giza pyramids nearby.", null, 30.044400m, 31.235700m },
                    { 14, "Alexandria", 4, "Mediterranean port city founded by Alexander the Great.", null, 31.200100m, 29.918700m },
                    { 15, "Luxor", 4, "Open-air museum of ancient Egyptian temples.", null, 25.687200m, 32.639600m },
                    { 16, "Sharm El Sheikh", 4, "Red Sea resort town.", null, 27.915800m, 34.330000m },
                    { 17, "Dubai", 5, "Global city known for skyscrapers and shopping.", null, 25.204800m, 55.270800m },
                    { 18, "Abu Dhabi", 5, "The capital of the UAE.", null, 24.453900m, 54.377300m },
                    { 19, "Sharjah", 5, "Cultural capital of the UAE.", null, 25.346300m, 55.420900m },
                    { 20, "Istanbul", 6, "Transcontinental city spanning Europe and Asia.", null, 41.008200m, 28.978400m },
                    { 21, "Ankara", 6, "The capital of Turkey.", null, 39.933400m, 32.859700m },
                    { 22, "Antalya", 6, "Mediterranean resort city on the Turkish Riviera.", null, 36.896900m, 30.713300m },
                    { 23, "Cappadocia", 6, "Famous for fairy chimneys and hot-air balloons.", null, 38.643100m, 34.828900m },
                    { 24, "Riyadh", 7, "The capital of Saudi Arabia.", null, 24.713600m, 46.675300m },
                    { 25, "Jeddah", 7, "Red Sea port city and gateway to Mecca.", null, 21.485800m, 39.192500m },
                    { 26, "Mecca", 7, "The holiest city in Islam.", null, 21.389100m, 39.857900m },
                    { 27, "Medina", 7, "The second holiest city in Islam.", null, 24.524700m, 39.569200m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Cities");
        }
    }
}
