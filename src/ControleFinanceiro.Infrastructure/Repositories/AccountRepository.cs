using ControleFinanceiro.Application.DTOs.Account;
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
    public class AccountRepository :IAccountRepository {
        
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext dbContext) { 
            _context = dbContext;
        }
        public async Task AddAsync(Account account) {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(Account account) {
            account.IsDeleted = true;
            _context.Accounts.Update(account);
            return _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountResponseDTO>> GetAccountWithBalanceAsync(Guid userId, DateTime dateTimeReference) {
            var query = from account in _context.Accounts
                        where account.UserId == userId && !account.IsDeleted
                        join transaction in _context.Transactions
                            .Where(t =>
                                t.UserId == userId &&
                                !t.IsDeleted &&
                                t.Date <= dateTimeReference)
                        on account.Id equals transaction.AccountId into transactionGroup
                        from tg in transactionGroup.DefaultIfEmpty()
                        group tg by new {
                            account.Id,
                            account.Name,
                            account.InitialBalance
                        }
        into g
                        select new AccountResponseDTO {
                            Id = g.Key.Id,
                            Name = g.Key.Name,
                            InitialBalance = g.Key.InitialBalance,
                            CurrentBalance = g.Key.InitialBalance +
                                g.Sum(x =>
                                    x != null
                                        ? (x.Type == Domain.Enums.TransactionType.Income ? x.Amount : -x.Amount)
                                        : 0)
                        };

            return await query.OrderBy(e => e.Name).ToListAsync();
        }

        public async Task<Account> GetByIdAsync(Guid id, Guid userId) {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && !a.IsDeleted);
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId) {     
            var accounts = await _context.Accounts.Where(a => a.UserId == userId && !a.IsDeleted).ToListAsync<Account>();
            return accounts;
        }

        public async Task<decimal> GetSumInitialBalanceAsync(Guid userId) {
            var sumInitialBalance = await _context.Accounts.Where(a => a.UserId == userId && !a.IsDeleted).SumAsync(a => (decimal?)a.InitialBalance) ?? 0;

            return sumInitialBalance;
        }

        public async Task UpdateAsync(Account account) {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}
