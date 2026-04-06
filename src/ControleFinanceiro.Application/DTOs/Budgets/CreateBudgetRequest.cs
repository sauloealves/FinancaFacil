namespace ControleFinanceiro.Application.DTOs.Budgets;

public class CreateBudgetRequest
{
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
}