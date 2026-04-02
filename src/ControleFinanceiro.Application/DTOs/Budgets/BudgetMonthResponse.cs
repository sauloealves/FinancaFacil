namespace ControleFinanceiro.Application.DTOs.Budgets;

public class BudgetMonthResponse
{
    public Guid Id { get; set; }
    public Guid BudgetId { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public List<BudgetItemDetailResponse> Items { get; set; } = new();
    public decimal TotalPlanned { get; set; }
    public decimal TotalRealized { get; set; }
    public decimal Difference { get; set; }
}