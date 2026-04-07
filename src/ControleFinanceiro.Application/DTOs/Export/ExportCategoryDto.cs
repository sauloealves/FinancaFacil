namespace ControleFinanceiro.Application.DTOs.Export;

public class ExportCategoryDto
{
    public string Nome { get; set; } = string.Empty;
    public string? CategoriaPai { get; set; }
}