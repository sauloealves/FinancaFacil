using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.UseCases;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers {

    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase {

        private readonly CategoryUseCase _categoryUseCase;
        public CategoryController(CategoryUseCase categoryUseCase) {
            _categoryUseCase = categoryUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request) {
            await _categoryUseCase.AddAsync(request.UserId, request.Name, request.ParentCategoryId);
            return Ok();
        }
    }
}
