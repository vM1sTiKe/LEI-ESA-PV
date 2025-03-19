using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class CartPlacedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "PlacedDate",
                table: "Cart",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_Cart_RestaurantId",
                table: "Cart",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Restaurant_RestaurantId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_RestaurantId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "PlacedDate",
                table: "Cart");
        }
    }
}
