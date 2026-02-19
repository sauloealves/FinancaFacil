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
            throw new NotImplementedException();
        }

        public async Task<Account> GetByIdAsync(Guid id, Guid userId) {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId && !a.IsDeleted);
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId) {            
            return await _context.Accounts.Where(a => a.UserId == userId && !a.IsDeleted).ToListAsync<Account>();
        }

        public async Task UpdateAsync(Account account) {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}
