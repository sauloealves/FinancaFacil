using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases {
    public class CategoryUseCase {
        private readonly ICategoryRepository _repository;
        public CategoryUseCase(ICategoryRepository repository) {
            _repository = repository;
        }
        public async Task AddAsync(Guid userId, string name, Guid? parentCategoryId) {
            var category = new Category(userId, name, parentCategoryId);
            await _repository.AddAsync(category);
        }
    }
}
