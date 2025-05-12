using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetDatabase.Migrations
{
    /// <inheritdoc />
    public partial class addWalletBalanceBoost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BalanceBoost",
                table: "Wallets",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceBoost",
                table: "Wallets");
        }
    }
}
