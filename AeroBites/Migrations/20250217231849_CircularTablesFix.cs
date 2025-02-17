using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class CircularTablesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Restaurant_RestaurantId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_RestaurantId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Item");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Item",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_RestaurantId",
                table: "Item",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Restaurant_RestaurantId",
                table: "Item",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id");
        }
    }
}
