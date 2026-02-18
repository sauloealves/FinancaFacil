using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Infrastructure.Persistence;

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
    }
}
