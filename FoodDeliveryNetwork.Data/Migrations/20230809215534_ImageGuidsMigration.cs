using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryNetwork.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImageGuidsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageGuid",
                table: "Restaurants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageGuid",
                table: "Dishes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageGuid",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ImageGuid",
                table: "Dishes");
        }
    }
}
