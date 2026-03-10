using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTOs.Transaction {
    public class TransactionResponse {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }
        public Guid? CategoryId { get; set; }

        public decimal Value { get; set; }

        public TransactionType Type { get; set; }

        public DateTime Date { get; set; }

        public string Description { get; set; } = null!;

        public OccurrenceType OccurrenceType { get; set; }

        public Guid? OccurrenceGroupId { get; set; }

        public int? InstallmentNumber { get; set; }
        public int? InstallmentTotal { get; set; }
    }
}
