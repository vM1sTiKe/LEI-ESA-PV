using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantPayPalAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_RestaurantId",
                table: "PaymentMethod",
                column: "RestaurantId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethod_Restaurant_RestaurantId",
                table: "PaymentMethod",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethod_Restaurant_RestaurantId",
                table: "PaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethod_RestaurantId",
                table: "PaymentMethod");
        }
    }
}
