using ControleFinanceiro.Application.Common.Exceptions;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Categories {
    public class DeleteCategoryUseCase {
        private readonly ICategoryRepository _repository;
        public DeleteCategoryUseCase(ICategoryRepository repository) {
            _repository = repository;
        }
        public async Task DeleteAsync(Guid categoryId, Guid userId) {
            var category = await _repository.GetByIdAsync(categoryId, userId);
            if (category == null) {
                throw new BusinessException("Categoria não encontrada ou acesso negado.");
            }
            
            if(await _repository.HasTransactions(categoryId, userId))
                throw new BusinessException("Não é possível excluir uma categoria que possui transações.");

            category.Delete(true);

            await _repository.UpdateAsync(category, userId);    
        }
    }
}
