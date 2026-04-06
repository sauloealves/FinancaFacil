namespace ControleFinanceiro.Application.DTOs.Budgets;

public class BudgetSummaryResponse
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal TotalPlanned { get; set; }
    public decimal TotalRealized { get; set; }
    public decimal Balance { get; set; }
    public decimal PercentageUsed { get; set; }
    public List<CategoryBreakdown> CategoryBreakdowns { get; set; } = new();
}

public class CategoryBreakdown
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
    public decimal RealizedAmount { get; set; }
    public decimal Difference { get; set; }
    public decimal PercentageUsed { get; set; }
}