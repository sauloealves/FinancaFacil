namespace ControleFinanceiro.Application.DTOs.Transaction {
    public class GetTransactionsFilter {
        public Guid? AccountId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? OccurrenceGroupId { get; set; }
    }   
}
