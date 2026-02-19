using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Repositories {
    public class TransactionRepository :ITransactionRepository {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context) {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction) {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Transaction> transactions) {
            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetAccountBalanceAsync(Guid accountId, Guid userId, DateTime referenceDate) {
            var result = await _context.Transactions
                            .Where(t =>
                                t.AccountId == accountId &&
                                t.UserId == userId &&
                                !t.IsDeleted &&
            t.Date <= referenceDate)
                            .GroupBy(_ => 1)
                            .Select(g => new {
                                TotalReceitas = g
                                    .Where(t => t.Type == Domain.Enums.TransactionType.Receita)
                                    .Sum(t => (decimal?)t.Amount) ?? 0,

                                TotalDespesas = g
                                    .Where(t => t.Type == Domain.Enums.TransactionType.Despesa)
                                    .Sum(t => (decimal?)t.Amount) ?? 0
                            })
                            .FirstOrDefaultAsync();

                                if(result == null)
                                    return 0;

                                return result.TotalReceitas - result.TotalDespesas;
        }
    }
}
