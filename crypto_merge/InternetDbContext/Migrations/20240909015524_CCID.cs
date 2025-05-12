using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetDatabase.Migrations
{
    /// <inheritdoc />
    public partial class CCID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberCart",
                table: "Wallets",
                newName: "NumberCard");

            migrationBuilder.AddColumn<string>(
                name: "CryptoCardId",
                table: "Wallets",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CryptoCardId",
                table: "Wallets");

            migrationBuilder.RenameColumn(
                name: "NumberCard",
                table: "Wallets",
                newName: "NumberCart");
        }
    }
}
