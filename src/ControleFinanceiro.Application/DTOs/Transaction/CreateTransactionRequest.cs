using ControleFinanceiro.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.DTOs.Transaction {
    public class CreateTransactionRequest {
        public string Description { get; set; } = null!;
        public decimal Value { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public OccurrenceType OccurrenceType { get; set; }
        public int? InstallmentFrom { get; set; }
        public int? InstallmentTo { get; set; }
        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public RecurrenceType Recurrence { get; set; }
        public DateTime? EndDate { get; set; }
        public EntryType Type { get; set; }
    }
}
