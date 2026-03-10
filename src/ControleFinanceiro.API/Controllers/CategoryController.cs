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
        public CategoryController(CreateCategoryUseCase categoryUseCase) {
            _categoryUseCase = categoryUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var getUseCase = HttpContext.RequestServices.GetService<GetCategoriesUseCase>();
            var categories = await getUseCase.GetAllByUserIdAsync(userId);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request) {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            await _categoryUseCase.AddAsync(userId,request.Name, request.ParentId);
            return Ok();
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> Update(Guid categoryId, UpdateCategoryRequest request) {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            var updateUseCase = HttpContext.RequestServices.GetService<UpdateCategoryUseCase>();
            await updateUseCase.UpdateAsync(categoryId,userId, request.Name, request.ParentId);
            return Ok();
        }

        [HttpDelete("{categoryID}")]
        public async Task<IActionResult> Delete(Guid categoryId) {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            var deleteUseCase = HttpContext.RequestServices.GetService<DeleteCategoryUseCase>();
            await deleteUseCase.DeleteAsync(categoryId, userId);
            return Ok();
        }
    }
}
