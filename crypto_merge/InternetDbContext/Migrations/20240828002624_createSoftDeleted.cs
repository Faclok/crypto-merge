using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetDatabase.Migrations
{
    /// <inheritdoc />
    public partial class createSoftDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Wallets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "TransactionWallets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "TransactionMessage",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Files",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Currencies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SoftDeleted",
                table: "Banks",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 1,
                column: "SoftDeleted",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "TransactionWallets");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "TransactionMessage");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "SoftDeleted",
                table: "Banks");
        }
    }
}
