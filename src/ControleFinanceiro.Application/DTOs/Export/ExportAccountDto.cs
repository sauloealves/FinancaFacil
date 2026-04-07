namespace ControleFinanceiro.Application.DTOs.Export;

public class ExportAccountDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal SaldoInicial { get; set; }
    public string Status { get; set; } = string.Empty;
}