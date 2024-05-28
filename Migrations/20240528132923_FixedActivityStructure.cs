using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFGetStarted.Migrations
{
    /// <inheritdoc />
    public partial class FixedActivityStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Accounts_AccountId",
                schema: "BankServer",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                schema: "BankServer",
                table: "Activities",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Accounts_AccountId",
                schema: "BankServer",
                table: "Activities",
                column: "AccountId",
                principalSchema: "BankServer",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Accounts_AccountId",
                schema: "BankServer",
                table: "Activities");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                schema: "BankServer",
                table: "Activities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Accounts_AccountId",
                schema: "BankServer",
                table: "Activities",
                column: "AccountId",
                principalSchema: "BankServer",
                principalTable: "Accounts",
                principalColumn: "Id");
        }
    }
}
