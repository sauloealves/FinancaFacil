using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class GetBudgetsUseCase
{
    private readonly IBudgetRepository _repository;

    public GetBudgetsUseCase(IBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BudgetResponse>> ExecuteAsync(Guid userId)
    {
        var budgets = await _repository.GetByUserIdAsync(userId);

        return budgets.Select(b => new BudgetResponse
        {
            Id = b.Id,
            Name = b.Name,
            Year = b.Year,
            Status = b.Status,
            UserId = b.UserId,
            CreatedAt = b.CreatedAt
        }).ToList();
    }
}