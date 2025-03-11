using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUnused : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountLog_Account_AccountId",
                table: "AccountLog");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_Account_OwnerId",
                table: "Restaurant");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "DropPointFavourite");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "DropPoint");

            migrationBuilder.DropIndex(
                name: "IX_Restaurant_OwnerId",
                table: "Restaurant");

            migrationBuilder.DropIndex(
                name: "IX_AccountLog_AccountId",
                table: "AccountLog");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "Delivered",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "signInDateTime",
                table: "AccountLog",
                newName: "SignInDateTime");

            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Cart");

            migrationBuilder.RenameColumn(
                name: "SignInDateTime",
                table: "AccountLog",
                newName: "signInDateTime");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Cart",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Delivered",
                table: "Cart",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DropPointFavourite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    DropPointId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_Restaurant_OwnerId",
                table: "Restaurant",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLog_AccountId",
                table: "AccountLog",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_AccountId",
                table: "Address",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DropPointFavourite_AccountId",
                table: "DropPointFavourite",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DropPointFavourite_DropPointId",
                table: "DropPointFavourite",
                column: "DropPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_AccountId",
                table: "Payment",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLog_Account_AccountId",
                table: "AccountLog",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_Account_OwnerId",
                table: "Restaurant",
                column: "OwnerId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
