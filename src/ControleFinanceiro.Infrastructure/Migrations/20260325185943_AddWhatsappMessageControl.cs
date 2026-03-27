using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWhatsappMessageControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FailedTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SuggestedType = table.Column<int>(type: "integer", nullable: true),
                    SuggestedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    SuggestedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SuggestedAccount = table.Column<Guid>(type: "uuid", nullable: true),
                    SuggestedCategory = table.Column<Guid>(type: "uuid", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FailedTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserKeywordMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Keyword = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKeywordMappings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FailedTransactions_CreatedAt",
                table: "FailedTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FailedTransactions_UserId",
                table: "FailedTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FailedTransactions_UserId_IsResolved",
                table: "FailedTransactions",
                columns: new[] { "UserId", "IsResolved" });

            migrationBuilder.CreateIndex(
                name: "IX_UserKeywordMappings_UserId_Keyword",
                table: "UserKeywordMappings",
                columns: new[] { "UserId", "Keyword" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FailedTransactions");

            migrationBuilder.DropTable(
                name: "UserKeywordMappings");
        }
    }
}
