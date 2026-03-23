using ControleFinanceiro.Application.DTOs.Invoice;
using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Invoice {
    public class CsvTextExtractor: ICsvTextExtractor {
        private readonly IAiParserService _aiParser;
        public CsvTextExtractor(IAiParserService aiParser) {
            _aiParser = aiParser;
        }
        public async Task<List<TransactionDto>> ExtractTextAsync(Stream csvStream) {
            using var reader = new StreamReader(csvStream);
            var csvContent = await reader.ReadToEndAsync();            

            var aiResponse = await _aiParser.ParseAsync(csvContent);

            var valid = aiResponse.Transactions
                        .Where(t => t.Amount != 0 && t.Date != default)
                        .ToList();

            return valid;
        }
    }
}
