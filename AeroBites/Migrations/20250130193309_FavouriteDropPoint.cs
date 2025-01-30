using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class FavouriteDropPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_DropPoint_DropPointId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_DropPointId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "DropPointId",
                table: "Account");

            migrationBuilder.CreateTable(
                name: "DropPointFavourite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DropPointId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropPointFavourite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DropPointFavourite_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DropPointFavourite_DropPoint_DropPointId",
                        column: x => x.DropPointId,
                        principalTable: "DropPoint",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DropPointFavourite_AccountId",
                table: "DropPointFavourite",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DropPointFavourite_DropPointId",
                table: "DropPointFavourite",
                column: "DropPointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DropPointFavourite");

            migrationBuilder.AddColumn<int>(
                name: "DropPointId",
                table: "Account",
                type: "int",
                nullable: true);

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
    }
}
