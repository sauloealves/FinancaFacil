using ControleFinanceiro.Application.DTOs;
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

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request) {
            await _categoryUseCase.AddAsync(request.UserId, request.Name, request.ParentCategoryId);
            return Ok();
        }
    }
}
