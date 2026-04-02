using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class AddCategoryToActiveBudgetsUseCase
{
    private readonly IBudgetRepository _budgetRepository;

    public AddCategoryToActiveBudgetsUseCase(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task ExecuteAsync(Guid userId, Guid categoryId)
    {
        await _budgetRepository.AddCategoryToActiveBudgetsAsync(userId, categoryId);
    }
}