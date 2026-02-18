using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.UseCases;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Runtime.InteropServices;

namespace ControleFinanceiro.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/accounts")]
    public class AccountController :ControllerBase {

        private readonly AccountUseCase _createUseCase;
        public AccountController(AccountUseCase createUseCase) {
            _createUseCase = createUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountRequest request) {
            await _createUseCase.AddAsync(request.UserId, request.Name, request.InitialBalance);
            return Ok();
        }
    }
}
