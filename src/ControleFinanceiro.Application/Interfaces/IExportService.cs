namespace ControleFinanceiro.Application.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportUserDataToExcelAsync(Guid userId);
}