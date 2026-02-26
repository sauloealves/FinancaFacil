using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.UseCases.Accounts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Runtime.InteropServices;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/accounts")]
    public class AccountController :ControllerBase {

        private readonly CreateAccountUseCase _createUseCase;
        private readonly GetAccountsUseCase _getAccountUseCase;
        public AccountController(CreateAccountUseCase createUseCase, GetAccountsUseCase getAccountUseCase    ) {
            _createUseCase = createUseCase;
            _getAccountUseCase = getAccountUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountRequest request) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _createUseCase.AddAsync(userId, request.Name, request.InitialBalance);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var accounts = await _getAccountUseCase.GetByUserIdAsync(userId);
            return Ok(accounts);
        }
    }
}
