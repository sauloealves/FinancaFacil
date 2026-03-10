using ControleFinanceiro.Application.DTOs.Category;
using ControleFinanceiro.Application.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Categories {
    public class GetCategoriesUseCase {
        private readonly ICategoryRepository _repository;
        public GetCategoriesUseCase(ICategoryRepository repository) {
            _repository = repository;
        }

        public async Task<List<CategoryResponseDTO>> GetAllByUserIdAsync(Guid userId) {
            var category = await _repository.GetAllByUserIdAsync(userId);
            if (category == null) {
                throw new Exception("Categoria não encontrada ou acesso negado.");
            }

            return category.Select(c => new CategoryResponseDTO {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentCategoryId,
                ParentName = category.FirstOrDefault(pc => pc.Id == c.ParentCategoryId)?.Name

            }).ToList();
        }
    }
}
