using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.DTOs.Transaction {
    public class TransactionDto {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
