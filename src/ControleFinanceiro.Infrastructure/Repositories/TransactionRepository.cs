using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.DTOs.Transactions;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;
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

        public async Task AddBatchAsync(Guid userId, List<Transaction> transactions) {                 
            await _context.Transactions.AddRangeAsync(transactions);
            _context.SaveChanges();
        }

        public async Task AddRangeAsync(List<Transaction> transactions) {
            await _context.Transactions.AddRangeAsync(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetBalanceAsync(Guid? accountId, Guid userId, DateTime referenceDate) {

            var query = _context.Transactions
                            .Where(t =>                                
                                t.UserId == userId &&
                                !t.IsDeleted &&
            t.Date <= referenceDate);

            if (accountId.HasValue)
                query = query.Where(t => t.AccountId == accountId.Value);

            var result = await query
                            .GroupBy(_ => 1)
                            .Select(g => new {
                                TotalReceitas = g
                                    .Where(t => t.Type == Domain.Enums.TransactionType.Income)
                                    .Sum(t => (decimal?)t.Amount) ?? 0,

                                TotalDespesas = g
                                    .Where(t => t.Type == Domain.Enums.TransactionType.Expense)
                                    .Sum(t => (decimal?)t.Amount) ?? 0
                            })
                            .FirstOrDefaultAsync();

                                if(result == null)
                                    return 0;

                                return result.TotalReceitas - result.TotalDespesas;
        }

        public async Task<List<Transaction>> GetAsync(Guid userId, Guid? accountId, DateTime? startDate, DateTime? endDate, Guid? occurrenceGroupId) {

            var query = _context.Transactions.Where(t => t.UserId == userId && !t.IsDeleted);

            if(accountId.HasValue)
                query = query.Where(t => t.AccountId == accountId.Value);

            if(startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);

            if(endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            if(occurrenceGroupId.HasValue)
                query = query.Where(t => t.OccurrenceGroupId == occurrenceGroupId.Value);

            return await query
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<Transaction> GetByIdAsync(Guid id, Guid userId) {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && !t.IsDeleted);
             return transaction;
        }

        public async Task<List<Transaction>> GetByGroupAsync(Guid groupId, Guid userId) {
            var transactions  = await _context.Transactions.Where(t => t.OccurrenceGroupId == groupId && t.UserId == userId && !t.IsDeleted).ToListAsync();

            return transactions;
        }

        public async Task SaveChangesAsync() {
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetExpenseByMonth(Guid userId, int month, int year) {
            return await _context.Transactions
        .Where(t =>
            t.UserId == userId &&
            t.Type == TransactionType.Expense &&
            t.Date.Month == month &&
            t.Date.Year == year && !t.IsDeleted)
        .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }

        public async Task<decimal> GetExpenseByCategory(Guid userId, Guid category) {
            return await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Type == TransactionType.Expense &&
                    t.CategoryId == category && !t.IsDeleted)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }

        public async Task<decimal> GetAccountBalanceAsync(Guid userId, Guid accountId) {
            return await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.AccountId == accountId &&
                    !t.IsDeleted)
                .GroupBy(_ => 1)
                .Select(g => new {
                    TotalReceitas = g
                        .Where(t => t.Type == TransactionType.Income)
                        .Sum(t => (decimal?)t.Amount) ?? 0,
                    TotalDespesas = g
                        .Where(t => t.Type == TransactionType.Expense)
                        .Sum(t => (decimal?)t.Amount) ?? 0
                })
                .FirstOrDefaultAsync() is var result && result != null
                    ? result.TotalReceitas - result.TotalDespesas
                    : 0;
        }

        public async Task<decimal> GetIncomeByMonth(Guid userId, int month, int year) {
            return await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Type == TransactionType.Income &&
                    t.Date.Month == month &&
                    t.Date.Year == year && !t.IsDeleted)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;
        }       

        public async Task<List<RecurringTransactionDetailResponse>> GetRecurringTransactionsAsync(
            Guid userId, 
            DateTime fromDate, 
            DateTime toDate)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .Join(
                    _context.Accounts.Where(a => !a.IsDeleted),
                    t => t.AccountId,
                    a => a.Id,
                    (t, a) => new { Transaction = t, Account = a })
                .GroupJoin(
                    _context.Categories.Where(c => !c.IsDeleted),
                    ta => ta.Transaction.CategoryId,
                    c => c.Id,
                    (ta, categories) => new { ta.Transaction, ta.Account, Category = categories.FirstOrDefault() })
                .Where(x => x.Transaction.CategoryId != null); // Somente com categoria

            // Filtro de data
            query = query.Where(x => x.Transaction.Date >= fromDate);
            query = query.Where(x => x.Transaction.Date <= toDate);

            var result = await query
                .OrderByDescending(x => x.Transaction.Date)
                .Select(x => new RecurringTransactionDetailResponse
                {
                    TransactionId = x.Transaction.Id,
                    Date = x.Transaction.Date,
                    Amount = x.Transaction.Amount,
                    Type = x.Transaction.Type,
                    TypeDescription = x.Transaction.Type == TransactionType.Income ? "Receita" :
                                     x.Transaction.Type == TransactionType.Expense ? "Despesa" : "Outro",
                    AccountName = x.Account.Name,
                    CategoryName = x.Category != null ? x.Category.Name : null,
                    Description = x.Transaction.Description,
                    OccurrenceType = x.Transaction.OccurrenceType,
                    OccurrenceGroupId = x.Transaction.OccurrenceGroupId,
                    InstallmentNumber = x.Transaction.InstallmentNumber,
                    InstallmentTotal = x.Transaction.InstallmentTotal
                })
                .ToListAsync();

            return result;
}
    }
}
