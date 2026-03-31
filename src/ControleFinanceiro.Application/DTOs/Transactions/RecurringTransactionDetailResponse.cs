using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.DTOs.Transactions;

public class RecurringTransactionDetailResponse
{
    public Guid TransactionId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string TypeDescription { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public string Description { get; set; } = string.Empty;
    public OccurrenceType OccurrenceType { get; set; }
    public Guid? OccurrenceGroupId { get; set; }
    public int? InstallmentNumber { get; set; }
    public int? InstallmentTotal { get; set; }
}