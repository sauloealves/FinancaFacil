using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Interfaces {
    public interface IAccountRepository {
        Task<Account> GetByIdAsync(Guid id, Guid userId);
        
        Task AddAsync(Account account);

        Task DeleteAsync(Account account);

        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
        
        Task UpdateAsync(Account account);

        Task<IEnumerable<AccountResponseDTO>> GetAccountWithBalanceAsync(Guid userid, DateTime dateTimeReference);
    }
}
