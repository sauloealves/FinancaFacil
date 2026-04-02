using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class SyncBudgetCategoriesUseCase
{
    private readonly IBudgetRepository _repository;

    public SyncBudgetCategoriesUseCase(IBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid userId, Guid budgetId)
    {
        // 1. Buscar o budget
        var budget = await _repository.GetByIdAsync(budgetId, userId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found");

        // 2. Buscar categorias ativas do usuário
        var allCategoryIds = await _repository.GetUserActiveCategoryIdsAsync(userId);

        // 3. Para cada mês, adicionar categorias que não existem
        foreach (var month in budget.Months)
        {
            var existingCategoryIds = month.Items.Select(i => i.CategoryId).ToHashSet();
            var missingCategoryIds = allCategoryIds.Where(cId => !existingCategoryIds.Contains(cId)).ToList();

            if (missingCategoryIds.Any())
            {
                month.InitializeCategories(missingCategoryIds);
            }
        }

        // 4. Salvar mudanças
        await _repository.SaveChangesAsync();
    }
}