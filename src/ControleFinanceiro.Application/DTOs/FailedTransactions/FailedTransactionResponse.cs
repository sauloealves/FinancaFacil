namespace ControleFinanceiro.Application.DTOs.FailedTransactions;

public class FailedTransactionResponse
{
    public Guid Id { get; set; }
    public string RawMessage { get; set; } = string.Empty;
    public Guid? AccountId { get; set; }
    public Guid? CategoryId{ get; set; }
    public decimal? Amount { get; set; }

}