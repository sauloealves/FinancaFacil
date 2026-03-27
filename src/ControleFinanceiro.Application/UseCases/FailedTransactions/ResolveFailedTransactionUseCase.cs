using ControleFinanceiro.Application.DTOs.FailedTransactions;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
namespace ControleFinanceiro.Application.UseCases.FailedTransactions;

public class ResolveFailedTransactionUseCase
{
    private readonly IFailedTransactionRepository _repository;

    public ResolveFailedTransactionUseCase(IFailedTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid userId, Guid id)
    {
        var failedTransaction = await _repository.GetByIdAsync(id, userId);

        if (failedTransaction == null)
            throw new InvalidOperationException("Failed transaction not found.");

        failedTransaction.Resolve(id);        

        await _repository.SaveChangesAsync();
    }
}