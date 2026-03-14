using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs.Auth;
using ControleFinanceiro.Application.UseCases.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/auth")]
    public class AuthController :ControllerBase {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly LoginUserUseCase _loginUserUseCase;
        private readonly ForgotPasswordUseCase _forgotPasswordUseCase;  
        private readonly ResetPasswordUseCase _resetPasswordUseCase;

        public AuthController(RegisterUserUseCase registerUserUseCase, LoginUserUseCase loginUserUseCase, ForgotPasswordUseCase forgotPasswordUseCase, ResetPasswordUseCase resetPasswordUseCase) {
            _registerUserUseCase = registerUserUseCase;
            _loginUserUseCase = loginUserUseCase;
            _forgotPasswordUseCase = forgotPasswordUseCase;
            _resetPasswordUseCase = resetPasswordUseCase;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request) {
            try {
                var result = await _loginUserUseCase.ExecuteAsync(request);
                return Ok(ApiResponse<LoginResponse>.Ok(result));
            } catch(InvalidOperationException ex) {
                return Unauthorized(ApiResponse.Fail(ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request) {
            try{
                await _forgotPasswordUseCase.ExecuteAsync(request);
            } catch(Exception ex) {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
            return Ok(ApiResponse.Ok());
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request) {
            await _resetPasswordUseCase.ExecuteAsync(request);
            return Ok(ApiResponse.Ok());
        }

        [HttpPost("teste")]        
        public async Task<IActionResult> Teste() {
            return Ok(ApiResponse<string>.Ok("Autenticado com sucesso!"));
        }
    }
}
