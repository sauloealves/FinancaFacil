using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Application.Interfaces;
using System.Globalization;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class GetBudgetMonthsUseCase
{
    private readonly IBudgetRepository _repository;

    public GetBudgetMonthsUseCase(IBudgetRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BudgetMonthResponse>> ExecuteAsync(Guid userId, Guid budgetId)
    {
        var budget = await _repository.GetByIdAsync(budgetId, userId);
        if (budget == null)
            throw new InvalidOperationException("Orçamento não encontrado");

        var months = await _repository.GetBudgetMonthsAsync(budgetId, userId);

        var responses = new List<BudgetMonthResponse>();

        foreach (var month in months)
        {
            var items = await _repository.GetBudgetItemsWithRealizationAsync(
                budgetId, 
                month.Month, 
                budget.Year, 
                userId);

            var monthName = new DateTime(budget.Year, month.Month, 1)
                .ToString("MMMM", new CultureInfo("pt-BR"));

            responses.Add(new BudgetMonthResponse
            {
                Id = month.Id,
                BudgetId = month.BudgetId,
                Month = month.Month,
                MonthName = char.ToUpper(monthName[0]) + monthName.Substring(1),
                Items = items,
                TotalPlanned = items.Sum(i => i.PlannedAmount),
                TotalRealized = items.Sum(i => i.RealizedAmount),
                Difference = items.Sum(i => i.Difference)
            });
        }

        return responses;
    }
}