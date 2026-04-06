using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTOs.Transaction {
    public class UpdateTransactionRequest {
        public string EditMode { get; set; } = "single";
        // single | fromThis | fromBeginning
        public string Description { get; set; } = null!;
        public decimal Value { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public OccurrenceType OccurrenceType { get; set; }
        public int? InstallmentFrom { get; set; }
        public int? InstallmentTo { get; set; }
        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public RecurrenceType Recurrence { get; set; }
        public DateTime? EndDate { get; set; }
        public EntryType Type { get; set; }        

        public bool? SendNotification { get; set; }
    }
}
