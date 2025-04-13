using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantOnDeleteNoOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "Restaurant",
                table: "Cart",
                newName: "RestaurantName");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_CartAddressId",
                table: "Cart",
                column: "CartAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_CartAddress_CartAddressId",
                table: "Cart",
                column: "CartAddressId",
                principalTable: "CartAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_CartAddress_CartAddressId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_CartAddressId",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "RestaurantName",
                table: "Cart",
                newName: "Restaurant");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
