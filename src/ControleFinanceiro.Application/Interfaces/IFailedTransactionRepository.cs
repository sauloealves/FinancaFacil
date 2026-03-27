using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Interfaces;

public interface IFailedTransactionRepository
{
    Task AddAsync(FailedTransaction failedTransaction);
    Task<FailedTransaction?> GetByIdAsync(Guid id, Guid userId);
    Task<List<FailedTransaction>> GetByUserIdAsync(Guid userId, bool includeResolved = false);
    Task SaveChangesAsync();
}
