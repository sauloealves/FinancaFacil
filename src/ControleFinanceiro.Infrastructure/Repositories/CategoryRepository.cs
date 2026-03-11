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
        public async Task<Category> GetByIdAsync(Guid id, Guid userId) {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id && c.UserId == userId);
            return category;
        }
        public async Task UpdateAsync(Category category, Guid userId) {
            
            await Task.Run(() => {
                var existingCategory = _context.Categories.FirstOrDefault(c => c.Id == category.Id && c.UserId == userId);
                if (existingCategory == null) {
                    throw new Exception("Categoria não encontrada ou ascesso negado.");
                }
                existingCategory.Update(category.Name, category.ParentCategoryId);
            });
            await _context.SaveChangesAsync();
        }        
        public async void DeleteAsync(Guid categoryId, Guid userId) {
            await Task.Run(() => {
                var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId && c.UserId == userId);
                if (category == null) {
                    throw new Exception("Categoria não encontrada ou ascesso negado.");
                }
                category.Delete(true);
            });
            await _context.SaveChangesAsync();
        }
        public async Task<List<Category>> GetAllByUserIdAsync(Guid userId) {
            return await Task.Run(() => _context.Categories.Where(c => c.UserId == userId && !c.IsDeleted).ToList());
        }

        public Task AddRangeAsync(List<Category> categories) {
            _context.Categories.AddRange(categories);
            return _context.SaveChangesAsync();
        }
    }
}
