using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;

public class InvoiceImportService :IInvoiceImportService {
    private readonly IPdfTextExtractor _pdfExtractor;
    private readonly IAiParserService _aiParser;
    private readonly ICsvTextExtractor _csvTextExtractor;

    public InvoiceImportService(
        IPdfTextExtractor pdfExtractor,
        IAiParserService aiParser,
        ICsvTextExtractor csvTextExtractor) {
        _pdfExtractor = pdfExtractor;
        _aiParser = aiParser;
        _csvTextExtractor = csvTextExtractor;
    }

    public async Task<List<TransactionDto>> ImportAsync(Stream stream, string extension) {
        switch(extension) {
             case
                ".pdf":
                    var text = await _pdfExtractor.ExtractTextAsync(stream);
                    if(string.IsNullOrWhiteSpace(text))
                        throw new Exception("Não foi possível extrair texto do PDF.");

                    var result = await _aiParser.ParseAsync(text);

                    var valid = result.Transactions
                        .Where(t => t.Amount != 0 && t.Date != default)
                        .ToList();

                    return valid;
            case ".csv":
                    return await _csvTextExtractor.ExtractTextAsync(stream);
            default:
                throw new NotSupportedException($"Extensão de arquivo '{extension}' não suportada.");
        }

        

        
    }    
}