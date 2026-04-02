namespace ControleFinanceiro.Application.DTOs.Budgets;

public class UpdateBudgetItemRequest
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }    
    public decimal PlannedAmount { get; set; }
}