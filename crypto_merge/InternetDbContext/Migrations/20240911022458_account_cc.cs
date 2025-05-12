using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternetDatabase.Migrations
{
    /// <inheritdoc />
    public partial class account_cc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountCryptoCardId",
                table: "Wallets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountCryptoCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SoftDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCryptoCards", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_AccountCryptoCardId",
                table: "Wallets",
                column: "AccountCryptoCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_AccountCryptoCards_AccountCryptoCardId",
                table: "Wallets",
                column: "AccountCryptoCardId",
                principalTable: "AccountCryptoCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_AccountCryptoCards_AccountCryptoCardId",
                table: "Wallets");

            migrationBuilder.DropTable(
                name: "AccountCryptoCards");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_AccountCryptoCardId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "AccountCryptoCardId",
                table: "Wallets");
        }
    }
}
