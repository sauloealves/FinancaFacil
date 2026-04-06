namespace ControleFinanceiro.Application.DTOs.Budgets;

public class UpdateBudgetItemsRequest
{
    public List<UpdateBudgetItemRequest> Items { get; set; } = new();
}