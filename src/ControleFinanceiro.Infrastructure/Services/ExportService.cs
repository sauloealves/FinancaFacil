using ClosedXML.Excel;
using ControleFinanceiro.Application.DTOs.Export;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infrastructure.Services;

public class ExportService : IExportService
{
    private readonly AppDbContext _context;

    public ExportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportUserDataToExcelAsync(Guid userId)
    {
        using var workbook = new XLWorkbook();

        // Aba 1: Contas
        await AddAccountsSheet(workbook, userId);

        // Aba 2: Categorias
        await AddCategoriesSheet(workbook, userId);

        // Aba 3: Transações
        await AddTransactionsSheet(workbook, userId);

        // Converter para byte array
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private async Task AddAccountsSheet(XLWorkbook workbook, Guid userId)
    {
        var accounts = await _context.Accounts
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .OrderBy(a => a.Name)
            .Select(a => new ExportAccountDto
            {
                Nome = a.Name,
                SaldoInicial = a.InitialBalance,
                Status = a.IsEnabled ? "Ativa" : "Inativa"
            })
            .ToListAsync();

        var worksheet = workbook.Worksheets.Add("Contas");

        // Cabeçalhos
        worksheet.Cell(1, 1).Value = "Nome";
        worksheet.Cell(1, 2).Value = "Saldo Inicial";
        worksheet.Cell(1, 3).Value = "Status";

        // Estilizar cabeçalho
        var headerRange = worksheet.Range(1, 1, 1, 3);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Dados
        if (accounts.Any())
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                var row = i + 2;
                worksheet.Cell(row, 1).Value = accounts[i].Nome;
                worksheet.Cell(row, 2).Value = accounts[i].SaldoInicial;
                worksheet.Cell(row, 3).Value = accounts[i].Status;
            }

            // Formatar coluna de valores
            worksheet.Column(2).Style.NumberFormat.Format = "R$ #,##0.00";
        }

        // Ajustar largura das colunas
        worksheet.Columns().AdjustToContents();
    }

    private async Task AddCategoriesSheet(XLWorkbook workbook, Guid userId)
    {
        var categories = await _context.Categories
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                c.Name,
                c.ParentCategoryId
            })
            .ToListAsync();

        // Buscar nomes das categorias pai
        var parentCategoryIds = categories
            .Where(c => c.ParentCategoryId.HasValue)
            .Select(c => c.ParentCategoryId!.Value)
            .Distinct()
            .ToList();

        var parentCategories = await _context.Categories
            .Where(c => parentCategoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var exportCategories = categories.Select(c => new ExportCategoryDto
        {
            Nome = c.Name,
            CategoriaPai = c.ParentCategoryId.HasValue && parentCategories.ContainsKey(c.ParentCategoryId.Value)
                ? parentCategories[c.ParentCategoryId.Value]
                : null
        }).ToList();

        var worksheet = workbook.Worksheets.Add("Categorias");

        // Cabeçalhos
        worksheet.Cell(1, 1).Value = "Nome";
        worksheet.Cell(1, 2).Value = "Categoria Pai";

        // Estilizar cabeçalho
        var headerRange = worksheet.Range(1, 1, 1, 2);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Dados
        if (exportCategories.Any())
        {
            for (int i = 0; i < exportCategories.Count; i++)
            {
                var row = i + 2;
                worksheet.Cell(row, 1).Value = exportCategories[i].Nome;
                worksheet.Cell(row, 2).Value = exportCategories[i].CategoriaPai ?? "";
            }
        }

        // Ajustar largura das colunas
        worksheet.Columns().AdjustToContents();
    }

    private async Task AddTransactionsSheet(XLWorkbook workbook, Guid userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .OrderByDescending(t => t.Date)
            .Select(t => new
            {
                t.Date,
                t.Description,
                t.Amount,
                t.Type,
                t.AccountId,
                t.CategoryId,
                t.OccurrenceType,
                t.InstallmentNumber,
                t.InstallmentTotal
            })
            .ToListAsync();

        // Buscar nomes das contas
        var accountIds = transactions.Select(t => t.AccountId).Distinct().ToList();
        var accounts = await _context.Accounts
            .Where(a => accountIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.Name);

        // Buscar nomes das categorias
        var categoryIds = transactions
            .Where(t => t.CategoryId.HasValue)
            .Select(t => t.CategoryId!.Value)
            .Distinct()
            .ToList();
        var categories = await _context.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var exportTransactions = transactions.Select(t => new ExportTransactionDto
        {
            Data = t.Date,
            Descricao = t.Description,
            Valor = t.Amount,
            Tipo = MapTransactionType(t.Type),
            Conta = accounts.ContainsKey(t.AccountId) ? accounts[t.AccountId] : "Desconhecida",
            Categoria = t.CategoryId.HasValue && categories.ContainsKey(t.CategoryId.Value)
                ? categories[t.CategoryId.Value]
                : null,
            TipoOcorrencia = MapOccurrenceType(t.OccurrenceType),
            NumeroParcela = t.InstallmentNumber,
            TotalParcelas = t.InstallmentTotal
        }).ToList();

        var worksheet = workbook.Worksheets.Add("Transações");

        // Cabeçalhos
        worksheet.Cell(1, 1).Value = "Data";
        worksheet.Cell(1, 2).Value = "Descrição";
        worksheet.Cell(1, 3).Value = "Valor";
        worksheet.Cell(1, 4).Value = "Tipo";
        worksheet.Cell(1, 5).Value = "Conta";
        worksheet.Cell(1, 6).Value = "Categoria";
        worksheet.Cell(1, 7).Value = "Tipo de Ocorrência";
        worksheet.Cell(1, 8).Value = "Parcela";
        worksheet.Cell(1, 9).Value = "Total de Parcelas";

        // Estilizar cabeçalho
        var headerRange = worksheet.Range(1, 1, 1, 9);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Dados
        if (exportTransactions.Any())
        {
            for (int i = 0; i < exportTransactions.Count; i++)
            {
                var row = i + 2;
                var transaction = exportTransactions[i];

                worksheet.Cell(row, 1).Value = transaction.Data;
                worksheet.Cell(row, 2).Value = transaction.Descricao;
                worksheet.Cell(row, 3).Value = transaction.Valor;
                worksheet.Cell(row, 4).Value = transaction.Tipo;
                worksheet.Cell(row, 5).Value = transaction.Conta;
                worksheet.Cell(row, 6).Value = transaction.Categoria ?? "";
                worksheet.Cell(row, 7).Value = transaction.TipoOcorrencia;
                worksheet.Cell(row, 8).Value = transaction.NumeroParcela.HasValue ? transaction.NumeroParcela.Value.ToString() : "";
                worksheet.Cell(row, 9).Value = transaction.TotalParcelas.HasValue ? transaction.TotalParcelas.Value.ToString() : "";

                // Colorir linha baseado no tipo
                if (transaction.Tipo == "Receita")
                {
                    worksheet.Row(row).Style.Font.FontColor = XLColor.DarkGreen;
                }
                else if (transaction.Tipo == "Despesa")
                {
                    worksheet.Row(row).Style.Font.FontColor = XLColor.DarkRed;
                }
            }

            // Formatar colunas
            worksheet.Column(1).Style.NumberFormat.Format = "dd/mm/yyyy";
            worksheet.Column(3).Style.NumberFormat.Format = "R$ #,##0.00";
        }

        // Ajustar largura das colunas
        worksheet.Columns().AdjustToContents();
    }

    private static string MapTransactionType(TransactionType type)
    {
        return type switch
        {
            TransactionType.Income => "Receita",
            TransactionType.Expense => "Despesa",
            TransactionType.Transfer => "Transferência",
            _ => "Outro"
        };
    }

    private static string MapOccurrenceType(OccurrenceType type)
    {
        return type switch
        {
            OccurrenceType.Single => "Única",
            OccurrenceType.Installment => "Parcelada",
            OccurrenceType.Recurring => "Recorrente",
            _ => "Outro"
        };
    }
}