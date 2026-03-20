using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs.Account;
using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.UseCases.Accounts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Runtime.InteropServices;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {

        private readonly CreateAccountUseCase _createUseCase;
        private readonly GetAccountsUseCase _getAccountUseCase;
        private readonly DeleteAccountUseCase _deleteAccountUseCase;
        private readonly UpdateAccountUseCase _updateAccountUseCase;
        public AccountController(CreateAccountUseCase createUseCase, GetAccountsUseCase getAccountUseCase, DeleteAccountUseCase deleteAccountUseCase, UpdateAccountUseCase updateAccountUseCase ) {
            _createUseCase = createUseCase;
            _getAccountUseCase = getAccountUseCase;
            _deleteAccountUseCase = deleteAccountUseCase;
            _updateAccountUseCase = updateAccountUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountRequest request)
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _createUseCase.AddAsync(userId, request.Name, request.InitialBalance);
            return Ok(ApiResponse.Ok());
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var accounts = await _getAccountUseCase.GetByUserIdAsync(userId);
            
            return Ok(ApiResponse<IEnumerable<AccountResponseDTO>>.Ok(accounts));
        }

        [HttpDelete("{accountId}")]
        public async Task<IActionResult> Delete(Guid accountId)
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _deleteAccountUseCase.DeleteAsync(accountId, userId);
            return Ok(ApiResponse.Ok());
        }

        [HttpPut("{accountId}")]
        public async Task<IActionResult> Update(Guid accountId, UpdateAccountRequest request)
        {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _updateAccountUseCase.UpdateAsync(accountId, userId, request.Name, request.InitialBalance, request.IsEnabled);
            return Ok(ApiResponse.Ok());
        }
    }
}