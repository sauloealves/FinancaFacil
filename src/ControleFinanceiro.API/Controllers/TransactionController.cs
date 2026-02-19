using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.UseCases.Transactions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers {
    
    [Authorize]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController :ControllerBase {
        private readonly CreateTransactionUseCase _createUseCase;

        public TransactionsController(CreateTransactionUseCase createUseCase) {
            _createUseCase = createUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionRequest request) {
            var userId = Guid.Parse(User.FindFirst("sub")!.Value);

            await _createUseCase.AddAsync(userId, request);

            return Ok();
        }
    }
}

