using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.DTOs {
    public class CreateTransactionRequest {
        public Guid AccountId { get; set; }
        public Guid? DestinationAccountId { get; set; }

        public Guid? CategoryId { get; set; }

        public decimal Amount { get; set; }
        public int Type { get; set; }
        public DateTime Date { get; set; }

        public string Description { get; set; } = null!;
    }
}
