using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Infrastructure.Persistence;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Repositories {
    public class CategoryRepository :ICategoryRepository {

        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) {
            _context = context;
        }

        public async Task AddAsync(Category category) {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public Task<Category> GetByIdAsync(Guid id) {
            throw new NotImplementedException();
        }
    }
}
