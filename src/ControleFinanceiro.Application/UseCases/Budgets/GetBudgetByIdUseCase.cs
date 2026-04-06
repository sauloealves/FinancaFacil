using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class GetBudgetByIdUseCase
{
    private readonly IBudgetRepository _repository;

    public GetBudgetByIdUseCase(IBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task<BudgetResponse?> ExecuteAsync(Guid userId, Guid budgetId)
    {
        var budget = await _repository.GetByIdAsync(budgetId, userId);

        if (budget == null)
            return null;

        return new BudgetResponse
        {
            Id = budget.Id,
            Name = budget.Name,
            Year = budget.Year,
            Status = budget.Status,
            UserId = budget.UserId,
            CreatedAt = budget.CreatedAt
        };
    }
}