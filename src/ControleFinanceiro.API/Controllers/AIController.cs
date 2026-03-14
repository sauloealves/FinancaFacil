using ControleFinanceiro.Application.AI;
using ControleFinanceiro.Application.Common;

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

        public AIController(AiIntentService intentService, FinancialQueryService financialQueryService) {
            _intentService = intentService;
            _queryService = financialQueryService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string query) {
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var intent = await _intentService.ParseIntent(query);
            var result = await _queryService.Execute(userId, intent);
            var response = await _intentService.AIResult(query,result);
            return Ok(ApiResponse<string>.Ok(response));
        }
    }
}
