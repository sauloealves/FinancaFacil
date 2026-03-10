using ControleFinanceiro.Application.DTOs.Account;
using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.UseCases.Transactions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers {
    
    [Authorize]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController :ControllerBase {
        private readonly CreateTransactionUseCase _createUseCase;
        private readonly GetTransactionsUseCase _getUseCase;
        private readonly GetBalanceUseCase _getBalanceUseCase;

        public TransactionsController(CreateTransactionUseCase createUseCase,GetTransactionsUseCase getTransactionsUseCase, GetBalanceUseCase getBalanceUseCase ) {
            _createUseCase = createUseCase;
            _getUseCase = getTransactionsUseCase;
            _getBalanceUseCase = getBalanceUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionRequest request) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await _createUseCase.AddAsync(userId, request);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetTransactionsFilter filter) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _getUseCase.ExecuteAsync(userId, filter);

            return Ok(result);
        }

        [HttpGet("GetBalance")]
        public async Task<IActionResult> GetBalance([FromQuery] GetBalanceRequest request) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _getBalanceUseCase.ExecuteAsync(userId, request);
            return Ok(result);
        }

        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Update(Guid transactionId, UpdateTransactionRequest request) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var updateUseCase = HttpContext.RequestServices.GetService<UpdateTransactionUseCase>();
            await updateUseCase.ExecuteAsync(transactionId, userId,request);
            return Ok();

        }
    }
}

