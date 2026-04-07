using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Export;

public class ExportUserDataUseCase
{
    private readonly IExportService _exportService;

    public ExportUserDataUseCase(IExportService exportService)
    {
        _exportService = exportService;
    }

    public async Task<byte[]> ExecuteAsync(Guid userId)
    {
        return await _exportService.ExportUserDataToExcelAsync(userId);
    }
}