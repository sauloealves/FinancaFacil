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
    public class UserRepository :IUserRepository {

        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) {
            _context = context;
        }
        public async Task<User?> GetByEmailAsync(string email) {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.Value == email.ToLower());
        }

        public async Task AddAsync(User user) {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
