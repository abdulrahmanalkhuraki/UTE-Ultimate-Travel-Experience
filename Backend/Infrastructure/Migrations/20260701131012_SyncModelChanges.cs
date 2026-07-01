using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "AttractionCategories",
                keyColumn: "CategoryId",
                keyValue: 24);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
