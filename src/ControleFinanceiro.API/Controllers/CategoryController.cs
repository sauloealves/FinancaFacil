using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs.Category;
using ControleFinanceiro.Application.UseCases.Categories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers {

    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase {

        private readonly CreateCategoryUseCase _categoryUseCase;
        private readonly GetCategoriesUseCase _getCategoriesUseCase;
        private readonly UpdateCategoryUseCase _updateCategoryUseCase;
        private readonly DeleteCategoryUseCase _deleteCategoryUseCase;

        public CategoryController(CreateCategoryUseCase categoryUseCase, GetCategoriesUseCase getCategoriesUseCase, UpdateCategoryUseCase updateCategoryUseCase, DeleteCategoryUseCase deleteCategoryUseCase) {
            _categoryUseCase = categoryUseCase;
            _getCategoriesUseCase = getCategoriesUseCase;
            _updateCategoryUseCase = updateCategoryUseCase;
            _deleteCategoryUseCase = deleteCategoryUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var categories = await _getCategoriesUseCase.GetAllByUserIdAsync(userId);
            return Ok(ApiResponse<IEnumerable<CategoryResponseDTO>>.Ok(categories));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request) {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            await _categoryUseCase.AddAsync(userId,request.Name, request.ParentId);
            return Ok(ApiResponse.Ok());
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> Update(Guid categoryId, UpdateCategoryRequest request) {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            await _updateCategoryUseCase.UpdateAsync(categoryId, userId, request.Name, request.ParentId);
            return Ok(ApiResponse.Ok());
        }

        [HttpDelete("{categoryID}")]
        public async Task<IActionResult> Delete(Guid categoryId) {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            await _deleteCategoryUseCase.DeleteAsync(categoryId, userId);
            return Ok(ApiResponse.Ok());
        }
    }
}
