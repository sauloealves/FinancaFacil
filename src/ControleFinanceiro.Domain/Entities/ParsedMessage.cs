using ControleFinanceiro.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Entities {
    public class ParsedMessage {
        public string RawText { get; set; }

        public TransactionType? Type { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }

        public string? AccountText { get; set; }
        public string? DestinationAccountText { get; set; }

        public string? CategoryText { get; set; }

        public double Confidence { get; set; } = 1.0;
    }
}
