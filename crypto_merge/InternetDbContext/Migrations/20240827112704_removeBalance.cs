using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetDatabase.Migrations
{
    /// <inheritdoc />
    public partial class removeBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Wallets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Wallets",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
