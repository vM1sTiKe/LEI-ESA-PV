using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroBites.Migrations
{
    /// <inheritdoc />
    public partial class AccountLogChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "AccountLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AccountLog_AccountId",
                table: "AccountLog",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountLog_Account_AccountId",
                table: "AccountLog",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountLog_Account_AccountId",
                table: "AccountLog");

            migrationBuilder.DropIndex(
                name: "IX_AccountLog_AccountId",
                table: "AccountLog");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AccountLog");
        }
    }
}
