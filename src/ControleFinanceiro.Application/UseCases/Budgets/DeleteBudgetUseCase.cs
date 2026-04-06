using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class DeleteBudgetUseCase
{
    private readonly IBudgetRepository _repository;

    public DeleteBudgetUseCase(IBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid userId, Guid budgetId)
    {
        var budget = await _repository.GetByIdAsync(budgetId, userId);

        if (budget == null)
            throw new InvalidOperationException("O orçamento não foi encontrado");

        await _repository.DeleteAsync(budget);
    }
}