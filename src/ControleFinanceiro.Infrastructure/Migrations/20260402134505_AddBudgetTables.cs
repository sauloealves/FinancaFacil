using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetMonths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetMonths_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BudgetMonthId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlannedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetItems_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetItems_BudgetMonths_BudgetMonthId",
                        column: x => x.BudgetMonthId,
                        principalTable: "BudgetMonths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_CategoryId_Date",
                table: "Transactions",
                columns: new[] { "UserId", "CategoryId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId_Date",
                table: "Transactions",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_AccountId",
                table: "BudgetItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetMonthId",
                table: "BudgetItems",
                column: "BudgetMonthId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetMonthId_CategoryId",
                table: "BudgetItems",
                columns: new[] { "BudgetMonthId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_CategoryId",
                table: "BudgetItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMonths_BudgetId",
                table: "BudgetMonths",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMonths_BudgetId_Month",
                table: "BudgetMonths",
                columns: new[] { "BudgetId", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId",
                table: "Budgets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_UserId_Year",
                table: "Budgets",
                columns: new[] { "UserId", "Year" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetItems");

            migrationBuilder.DropTable(
                name: "BudgetMonths");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId_CategoryId_Date",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId_Date",
                table: "Transactions");
        }
    }
}
