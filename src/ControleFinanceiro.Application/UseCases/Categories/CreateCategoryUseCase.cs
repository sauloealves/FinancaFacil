using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases.Budgets;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.UseCases.Categories;

public class CreateCategoryUseCase
{
    private readonly ICategoryRepository _repository;
    private readonly AddCategoryToActiveBudgetsUseCase _addCategoryToActiveBudgetsUseCase;

    public CreateCategoryUseCase(
        ICategoryRepository repository,
        AddCategoryToActiveBudgetsUseCase addCategoryToActiveBudgetsUseCase)
    {
        _repository = repository;
        _addCategoryToActiveBudgetsUseCase = addCategoryToActiveBudgetsUseCase;
    }

    public async Task<Guid> AddAsync(Guid userId, string name, Guid? parentCategoryId = null)
    {
        // 1. Criar a categoria
        var category = new Category(userId, name, parentCategoryId);
        await _repository.AddAsync(category);

        // 2. Adicionar a categoria em todos os budgets ativos do usuário
        await _addCategoryToActiveBudgetsUseCase.ExecuteAsync(userId, category.Id);

        return category.Id;
    }
}
