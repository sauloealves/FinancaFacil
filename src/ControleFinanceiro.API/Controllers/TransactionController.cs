using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs.Account;
using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.UseCases.Transactions;
using ControleFinanceiro.Domain.Enums;

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
        private readonly DeleteTransactionUseCase _deleteUseCase;
        private readonly UpdateTransactionUseCase _updateUseCase;
        private readonly CreateBatchTransactionUseCase _createBatchUseCase;

        public TransactionsController(CreateTransactionUseCase createUseCase,GetTransactionsUseCase getTransactionsUseCase, GetBalanceUseCase getBalanceUseCase, DeleteTransactionUseCase deleteTransactionUseCase, UpdateTransactionUseCase updateTransactionUseCase, CreateBatchTransactionUseCase createBatchUseCase) {
            _createUseCase = createUseCase;
            _getUseCase = getTransactionsUseCase;
            _getBalanceUseCase = getBalanceUseCase;
            _deleteUseCase = deleteTransactionUseCase;
            _updateUseCase = updateTransactionUseCase;
            _createBatchUseCase = createBatchUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionRequest request) {
            try {
                Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _createUseCase.AddAsync(userId, request);
                return Ok(ApiResponse.Ok());
            } catch(Exception ex) {

                return BadRequest(ApiResponse.Fail($"Ocorreu um erro ao criar a transação. {ex.Message}"));
            }
            
        }

        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatch(List<CreateTransactionRequest> requests) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);            
            await _createBatchUseCase.ExecuteAsync(userId, requests);
            return Ok(ApiResponse.Ok());
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetTransactionsFilter filter) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _getUseCase.ExecuteAsync(userId, filter);

            return Ok(ApiResponse<List<TransactionResponse>>.Ok(result));
        }

        [HttpGet("GetBalance")]
        public async Task<IActionResult> GetBalance([FromQuery] GetBalanceRequest request) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _getBalanceUseCase.ExecuteAsync(userId, request);
            return Ok(ApiResponse<BalanceResponse>.Ok(result));
        }

        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Update(Guid transactionId, UpdateTransactionRequest request) {
            try {
                Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _updateUseCase.ExecuteAsync(transactionId, userId, request);
                return Ok(ApiResponse.Ok());
            } catch(Exception ex) {

                return BadRequest(ApiResponse.Fail($"Ocorreu um erro ao atualizar a transação. {ex.Message}"));
            }
            

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] DeleteScope scope ) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            await _deleteUseCase.ExecuteAsync(id, userId, scope);
            return Ok(ApiResponse.Ok());
        }
    }
}

