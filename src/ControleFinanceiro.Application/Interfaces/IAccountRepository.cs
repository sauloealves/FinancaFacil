using ControleFinanceiro.Application.DTOs.Account;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Interfaces {
    public interface IAccountRepository {
        Task<Account> GetByIdAsync(Guid id, Guid userId);
        
        Task AddAsync(Account account);

        Task DeleteAsync(Account account);

        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
        
        Task UpdateAsync(Account account);

        Task<IEnumerable<AccountResponseDTO>> GetAccountWithBalanceAsync(Guid userid, DateTime dateTimeReference);

        Task<decimal> GetSumInitialBalanceAsync(Guid userId, Guid? accountId);

        Task AddRangeAsync(List<Account> accounts);

        Task<bool> HasTransactions(Guid accountId, Guid userId);
    }
}
