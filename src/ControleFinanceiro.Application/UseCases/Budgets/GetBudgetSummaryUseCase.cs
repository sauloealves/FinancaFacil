using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class GetBudgetSummaryUseCase
{
    private readonly IBudgetRepository _repository;

    public GetBudgetSummaryUseCase(IBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task<BudgetSummaryResponse> ExecuteAsync(Guid userId, Guid budgetId)
    {
        var budget = await _repository.GetByIdAsync(budgetId, userId);

        if (budget == null)
            throw new InvalidOperationException("Orçamento não encontrado");

        return await _repository.GetBudgetSummaryAsync(budgetId, budget.Year, userId);
    }
}