using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Transactions {
    public class GetTransactionsUseCase {
        private readonly ITransactionRepository _repository;

        public GetTransactionsUseCase(ITransactionRepository repository) {
            _repository = repository;
        }

        public async Task<List<TransactionResponse>> ExecuteAsync(
            Guid userId,
            GetTransactionsFilter filter) {
            var transactions = await _repository.GetAsync(
                userId,
                filter.AccountId,
                filter.StartDate,
                filter.EndDate,
                filter.OccurrenceGroupId);

            return transactions.Select(t => new TransactionResponse {
                Id = t.Id,
                AccountId = t.AccountId,
                CategoryId = t.CategoryId,
                Value = t.Amount,
                Type = t.Type,
                Date = t.Date,
                Description = t.Description,
                OccurrenceType = t.OccurrenceType,
                OccurrenceGroupId = t.OccurrenceGroupId,
                InstallmentNumber = t.InstallmentNumber,
                InstallmentTotal = t.InstallmentTotal
            }).ToList();
        }
    }
}
