using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class KeepOrdersONRestDelete4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
