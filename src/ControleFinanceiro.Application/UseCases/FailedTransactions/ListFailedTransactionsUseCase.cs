using ControleFinanceiro.Application.DTOs.FailedTransactions;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.FailedTransactions;

public class ListFailedTransactionsUseCase
{
    private readonly IFailedTransactionRepository _repository;

    public ListFailedTransactionsUseCase(IFailedTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FailedTransactionResponse>> ExecuteAsync(Guid userId, bool includeResolved = false)
    {
        var failedTransactions = await _repository.GetByUserIdAsync(userId, includeResolved);

        return failedTransactions.Select(ft => new FailedTransactionResponse { 
            Id = ft.Id,
            Amount = ft.SuggestedAmount,
            AccountId = ft.SuggestedAccount,            
            CategoryId = ft.SuggestedCategory,            
            RawMessage = ft.OriginalText
        }).ToList();
    }
}