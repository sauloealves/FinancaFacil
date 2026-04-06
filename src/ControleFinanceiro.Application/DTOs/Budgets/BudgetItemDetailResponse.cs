namespace ControleFinanceiro.Application.DTOs.Budgets;

public class BudgetItemDetailResponse
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid? AccountId { get; set; }
    public string? AccountName { get; set; }
    public decimal PlannedAmount { get; set; }
    public decimal RealizedAmount { get; set; }
    public decimal Difference { get; set; }
    public decimal PercentageUsed { get; set; }
}