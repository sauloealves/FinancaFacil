using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.UseCases;

using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers {
    [ApiController]
    [Route("api/auth")]
    public class AuthController :ControllerBase {
        private readonly RegisterUserUseCase _registerUserUseCase;

        public AuthController(RegisterUserUseCase registerUserUseCase) {
            _registerUserUseCase = registerUserUseCase;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request) {
            try {
                var result = await _registerUserUseCase.ExecuteAsync(request);
                return Ok(result);
            } catch(InvalidOperationException ex) {
                return Conflict(new { message = ex.Message });
            } catch(ArgumentException ex) {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
