using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs.FailedTransactions;
using ControleFinanceiro.Application.UseCases.FailedTransactions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers;

[Authorize]
[ApiController]
[Route("api/failed-transactions")]
public class FailedTransactionsController : ControllerBase
{
    private readonly ResolveFailedTransactionUseCase _resolveUseCase;
    private readonly ListFailedTransactionsUseCase _listUseCase;

    public FailedTransactionsController(
        ResolveFailedTransactionUseCase resolveUseCase,
        ListFailedTransactionsUseCase listUseCase
    )
    {
        _resolveUseCase = resolveUseCase;
        _listUseCase = listUseCase;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }    

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = GetUserId();
        var failedTransactions = await _listUseCase.ExecuteAsync(userId, false);
        return Ok(ApiResponse<List<FailedTransactionResponse>>.Ok(failedTransactions));
    }    

    [HttpPut("{id}")]
    public async Task<IActionResult> Resolve(Guid id)
    {
        try
        {
            var userId = GetUserId();
            await _resolveUseCase.ExecuteAsync(userId, id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}