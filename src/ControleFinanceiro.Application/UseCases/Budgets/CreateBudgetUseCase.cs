using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class CreateBudgetUseCase
{
    private readonly IBudgetRepository _repository;

    public CreateBudgetUseCase(IBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task<BudgetResponse> ExecuteAsync(Guid userId, CreateBudgetRequest request)
    {
        // 1. Buscar categorias ativas do usuário
        var categoryIds = await _repository.GetUserActiveCategoryIdsAsync(userId);

        // 2. Criar o budget com 12 meses
        var budget = new Budget(userId, request.Name, request.Year);

        // 3. Adicionar todas as categorias em todos os meses com valor inicial 0
        if (categoryIds.Any())
        {
            budget.InitializeCategoriesForAllMonths(categoryIds);
        }

        // 4. Persistir no banco
        await _repository.AddAsync(budget);

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