using ControleFinanceiro.Application.DTOs.Transaction;

using System.Text.Json.Serialization;

namespace ControleFinanceiro.Application.DTOs.Invoice {
    public class InvoiceResult {
        [JsonPropertyName("transactions")]
        public List<TransactionDto> Transactions { get; set; }
    }
}
