using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleFinanceiro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Budgets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_IsDeleted",
                table: "Budgets",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Budgets_IsDeleted",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Budgets");
        }
    }
}
