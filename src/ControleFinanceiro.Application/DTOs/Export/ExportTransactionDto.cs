namespace ControleFinanceiro.Application.DTOs.Export;

public class ExportTransactionDto
{
    public DateTime Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Conta { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public string TipoOcorrencia { get; set; } = string.Empty;
    public int? NumeroParcela { get; set; }
    public int? TotalParcelas { get; set; }
}