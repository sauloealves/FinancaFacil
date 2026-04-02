using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTOs.Budgets;

public class BudgetResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public BudgetStatus Status { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
}