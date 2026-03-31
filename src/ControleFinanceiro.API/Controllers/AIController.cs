using ControleFinanceiro.Application.AI;
using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs.AI;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases.Transactions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers {
    [Authorize]
    [Route("api/ai")]
    [ApiController] 
    public class AIController :ControllerBase {

        private readonly AiIntentService _intentService;
        public readonly FinancialQueryService _queryService;
        private readonly GetTransactionsUseCase _getUseCase;
        private readonly ITransactionRepository _transactionRepo;

        public AIController(AiIntentService intentService, FinancialQueryService financialQueryService, GetTransactionsUseCase getUseCase, ITransactionRepository transactionRepo) {
            _intentService = intentService;
            _queryService = financialQueryService;
            _getUseCase = getUseCase;
            _transactionRepo = transactionRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string query) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var intent = await _intentService.ParseIntent(query);
            //var result = await _queryService.Execute(userId, intent);
            var result = await _getUseCase.ExecuteAsync(userId, new Application.DTOs.Transaction.GetTransactionsFilter() {EndDate = DateTime.Now, StartDate = DateTime.Now.AddMonths(-4), AccountId = null, OccurrenceGroupId = null});
            var response = await _intentService.AIResult(query,result);
            return Ok(ApiResponse<AIResponse>.Ok(new AIResponse {Descricao = response }));
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] string prompt) {
            var userId = GetUserId();   
            var data = await _intentService.GetPeriodAsync(prompt);
            var transactions = await _transactionRepo.GetRecurringTransactionsAsync(userId,data.StartDate,data.EndDate);
            var response = await _intentService.GenerateFinancialSummary(transactions,prompt);
            return Ok(ApiResponse<AIResponse>.Ok(new AIResponse { Descricao = response }));
        }

        private Guid GetUserId() {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
