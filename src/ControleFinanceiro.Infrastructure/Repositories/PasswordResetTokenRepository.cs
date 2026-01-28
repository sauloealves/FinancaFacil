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
    public class PasswordResetTokenRepository :IPasswordResetTokenRepository {
        private readonly AppDbContext _context;

        public PasswordResetTokenRepository(AppDbContext context) {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token) {
            await _context.Set<PasswordResetToken>().AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetToken?> GetValidAsync(string token) {
            return await _context.Set<PasswordResetToken>()
                .FirstOrDefaultAsync(t =>
                    t.Token == token &&
                    !t.Used &&
                    t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task UpdateAsync(PasswordResetToken token) {
            _context.Set<PasswordResetToken>().Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
