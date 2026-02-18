using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Infrastructure.Persistence;

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

        public Task<Account> GetByIdAsync(Guid id) {
            throw new NotImplementedException();
        }
    }
}
