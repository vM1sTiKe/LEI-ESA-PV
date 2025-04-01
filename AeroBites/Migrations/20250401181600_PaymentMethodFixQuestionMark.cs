using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class PaymentMethodFixQuestionMark : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethod_Restaurant_RestaurantId",
                table: "PaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethod_RestaurantId",
                table: "PaymentMethod");

            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "PaymentMethod",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "PaymentMethod",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_RestaurantId",
                table: "PaymentMethod",
                column: "RestaurantId",
                unique: true,
                filter: "[RestaurantId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethod_Restaurant_RestaurantId",
                table: "PaymentMethod",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id");
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

            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "PaymentMethod",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "PaymentMethod",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
