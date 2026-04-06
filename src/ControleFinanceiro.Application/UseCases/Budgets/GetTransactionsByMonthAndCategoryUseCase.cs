using ControleFinanceiro.Application.DTOs.Transactions;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Budgets;

public class GetTransactionsByMonthAndCategoryUseCase
{
    private readonly ITransactionRepository _repository;

    public GetTransactionsByMonthAndCategoryUseCase(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<RecurringTransactionDetailResponse>> ExecuteAsync(
        Guid userId, 
        int month, 
        int year,
        Guid? categoryId = null)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var transactions = await _repository.GetRecurringTransactionsAsync(userId, startDate, endDate);

        if (categoryId.HasValue)
        {
            transactions = transactions
                .Where(t => t.CategoryName != null && 
                           _repository.GetByIdAsync(t.TransactionId, userId).Result.CategoryId == categoryId.Value)
                .ToList();
        }

        return transactions;
    }
}