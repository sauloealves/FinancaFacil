using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Interfaces {
    public interface ITransactionRepository {
        Task AddAsync(Transaction transaction);
        Task AddRangeAsync(List<Transaction> transactions);
        Task<decimal> GetBalanceAsync(Guid? accountId, Guid userId, DateTime referenceDate);
        Task<List<Transaction>> GetAsync(Guid userId,Guid? accountId,DateTime? startDate,DateTime? endDate, Guid? occurrenceGroupId);
        
        Task<Transaction> GetByIdAsync(Guid id, Guid userId);
        Task<List<Transaction>> GetByGroupAsync(Guid groupId, Guid userId);
        Task SaveChangesAsync();
        Task<decimal> GetExpenseByMonth(Guid userId, int month, int year);

        Task<decimal> GetExpenseByCategory(Guid userId, Guid category);

        Task<decimal> GetAccountBalanceAsync(Guid userId, Guid accountId);

        Task<decimal> GetIncomeByMonth(Guid userId, int month, int year);              

    }
}
