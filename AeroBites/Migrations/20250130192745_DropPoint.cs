using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class DropPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DropPointId",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DropPoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropPoint", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_DropPointId",
                table: "Account",
                column: "DropPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_DropPoint_DropPointId",
                table: "Account",
                column: "DropPointId",
                principalTable: "DropPoint",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_DropPoint_DropPointId",
                table: "Account");

            migrationBuilder.DropTable(
                name: "DropPoint");

            migrationBuilder.DropIndex(
                name: "IX_Account_DropPointId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "DropPointId",
                table: "Account");
        }
    }
}
