using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Application.DTOs.Transactions;
using ControleFinanceiro.Application.UseCases.Budgets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly CreateBudgetUseCase _createBudgetUseCase;
    private readonly GetBudgetsUseCase _getBudgetsUseCase;
    private readonly GetBudgetByIdUseCase _getBudgetByIdUseCase;
    private readonly GetBudgetSummaryUseCase _getBudgetSummaryUseCase;
    private readonly GetBudgetMonthsUseCase _getBudgetMonthsUseCase;
    private readonly UpdateBudgetItemsUseCase _updateBudgetItemsUseCase;
    private readonly GetTransactionsByMonthAndCategoryUseCase _getTransactionsByMonthAndCategoryUseCase;
    private readonly DeleteBudgetUseCase _deleteBudgetUseCase; // Adicionar
    private readonly SyncBudgetCategoriesUseCase _syncBudgetCategoriesUseCase; // Novo

    public BudgetsController(
        CreateBudgetUseCase createBudgetUseCase,
        GetBudgetsUseCase getBudgetsUseCase,
        GetBudgetByIdUseCase getBudgetByIdUseCase,
        GetBudgetSummaryUseCase getBudgetSummaryUseCase,
        GetBudgetMonthsUseCase getBudgetMonthsUseCase,
        UpdateBudgetItemsUseCase updateBudgetItemsUseCase,
        GetTransactionsByMonthAndCategoryUseCase getTransactionsByMonthAndCategoryUseCase,
        DeleteBudgetUseCase deleteBudgetUseCase, // Adicionar
        SyncBudgetCategoriesUseCase syncBudgetCategoriesUseCase) // Novo
    {
        _createBudgetUseCase = createBudgetUseCase;
        _getBudgetsUseCase = getBudgetsUseCase;
        _getBudgetByIdUseCase = getBudgetByIdUseCase;
        _getBudgetSummaryUseCase = getBudgetSummaryUseCase;
        _getBudgetMonthsUseCase = getBudgetMonthsUseCase;
        _updateBudgetItemsUseCase = updateBudgetItemsUseCase;
        _getTransactionsByMonthAndCategoryUseCase = getTransactionsByMonthAndCategoryUseCase;
        _deleteBudgetUseCase = deleteBudgetUseCase; // Adicionar
        _syncBudgetCategoriesUseCase = syncBudgetCategoriesUseCase; // Novo
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Cria um novo orçamento anual
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBudgetRequest request)
    {
        var userId = GetUserId();
        var response = await _createBudgetUseCase.ExecuteAsync(userId, request);
        return Ok(ApiResponse<BudgetResponse>.Ok(response));
    }

    /// <summary>
    /// Lista todos os orçamentos do usuário
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var budgets = await _getBudgetsUseCase.ExecuteAsync(userId);
        return Ok(ApiResponse<List<BudgetResponse>>.Ok(budgets));
    }

    /// <summary>
    /// Busca orçamento por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var budget = await _getBudgetByIdUseCase.ExecuteAsync(userId, id);

        if (budget == null)
            return NotFound(ApiResponse<BudgetResponse>.Fail("Orçamento não encontrado"));

        return Ok(ApiResponse<BudgetResponse>.Ok(budget));
    }

    /// <summary>
    /// Retorna resumo consolidado do orçamento
    /// </summary>
    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var summary = await _getBudgetSummaryUseCase.ExecuteAsync(userId, id);
            return Ok(ApiResponse<BudgetSummaryResponse>.Ok(summary));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<BudgetSummaryResponse>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Lista todos os meses do orçamento com itens e valores realizados
    /// </summary>
    [HttpGet("{id}/months")]
    public async Task<IActionResult> GetMonths(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var months = await _getBudgetMonthsUseCase.ExecuteAsync(userId, id);
            return Ok(ApiResponse<List<BudgetMonthResponse>>.Ok(months));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<List<BudgetMonthResponse>>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Atualiza itens de orçamento (batch update)
    /// </summary>
    [HttpPut("items")]
    public async Task<IActionResult> UpdateItems([FromBody] UpdateBudgetItemRequest request)
    {
        try
        {
            var userId = GetUserId();
            await _updateBudgetItemsUseCase.ExecuteAsync(userId, request);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Exclui um orçamento (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            await _deleteBudgetUseCase.ExecuteAsync(userId, id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lista transações filtradas por mês e categoria
    /// </summary>
    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] Guid? categoryId = null)
    {
        var userId = GetUserId();
        var transactions = await _getTransactionsByMonthAndCategoryUseCase.ExecuteAsync(
            userId, month, year, categoryId);
        return Ok(ApiResponse<List<RecurringTransactionDetailResponse>>.Ok(transactions));
    }

    /// <summary>
    /// Sincroniza categorias novas com o orçamento existente
    /// </summary>
    [HttpPost("{id}/sync-categories")]
    public async Task<IActionResult> SyncCategories(Guid id)
    {
        try
        {
            var userId = GetUserId();
            await _syncBudgetCategoriesUseCase.ExecuteAsync(userId, id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}