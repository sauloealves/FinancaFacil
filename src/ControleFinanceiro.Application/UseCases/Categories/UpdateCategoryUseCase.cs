using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Categories {
    public class UpdateCategoryUseCase {
        private readonly ICategoryRepository _repository;
        public UpdateCategoryUseCase(ICategoryRepository repository) {
            _repository = repository;
        }
        public async Task UpdateAsync(Guid categoryId, Guid userId, string name, Guid? parentCategoryId = null) {
            var category = await _repository.GetByIdAsync(categoryId,userId);

            if (category == null || category.UserId != userId) {
                throw new Exception("Categoria não encontrada ou acesso negado.");
            }

            category.Update(name, parentCategoryId);
            await _repository.UpdateAsync(category, userId);
        }
    }
}
