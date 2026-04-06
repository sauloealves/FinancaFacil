using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class UpdateBudgetItemsUseCase
{
    private readonly IBudgetRepository _repository;

    public UpdateBudgetItemsUseCase(IBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid userId, UpdateBudgetItemRequest request)
    {
        var budgetItem = await _repository.GetBudgetItemAsync(request.Id, userId);
        if (budgetItem == null)
            throw new InvalidOperationException("Item do orçamento não encontrado");
        
        budgetItem.UpdatePlannedAmount(request.PlannedAmount);
        await _repository.UpdateBudgetItemAsync(budgetItem);
        await _repository.SaveChangesAsync();
    }
}