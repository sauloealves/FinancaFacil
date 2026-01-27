using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.UseCases;

using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers {
    [ApiController]
    [Route("api/auth")]
    public class AuthController :ControllerBase {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly LoginUserUseCase _loginUserUseCase;

        public AuthController(RegisterUserUseCase registerUserUseCase, LoginUserUseCase loginUserUseCase) {
            _registerUserUseCase = registerUserUseCase;
            _loginUserUseCase = loginUserUseCase;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request) {
            try {
                var result = await _loginUserUseCase.ExecuteAsync(request);
                return Ok(result);
            } catch(InvalidOperationException ex) {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
