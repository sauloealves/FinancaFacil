using ControleFinanceiro.Application.UseCases.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ControleFinanceiro.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly ExportUserDataUseCase _exportUserDataUseCase;

    public ExportController(ExportUserDataUseCase exportUserDataUseCase)
    {
        _exportUserDataUseCase = exportUserDataUseCase;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Exporta todos os dados do usuário em um arquivo Excel com múltiplas abas
    /// </summary>
    [HttpGet("backup")]
    public async Task<IActionResult> ExportBackup()
    {
        var userId = GetUserId();
        var fileBytes = await _exportUserDataUseCase.ExecuteAsync(userId);

        var fileName = $"backup-financeiro-{DateTime.Now:yyyy-MM-dd-HHmmss}.xlsx";

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }
}