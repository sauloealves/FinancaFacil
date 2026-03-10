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
                throw new Exception("Categoria não encontrada ou acesso negado.");
            }
            category.Delete(true);

            await _repository.UpdateAsync(category, userId);    
        }
    }
}
