using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class AddressFixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Restaurant");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Address",
                newName: "IsActive");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Address",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Address");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Address",
                newName: "isActive");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Restaurant",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
