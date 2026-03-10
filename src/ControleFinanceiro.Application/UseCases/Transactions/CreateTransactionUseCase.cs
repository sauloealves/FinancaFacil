using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.UseCases.Transactions {
    public class CreateTransactionUseCase {
        private readonly ITransactionRepository _repository;

        public CreateTransactionUseCase(ITransactionRepository repository) {
            _repository = repository;
        }

        public async Task AddAsync(Guid userId, CreateTransactionRequest request) {
            var transactionType = MapEntryType(request.Type);

            if(IsTransfer(request)) {
                await HandleTransfer(userId, request, transactionType);
                return;
            }

            switch(request.OccurrenceType) {
                case OccurrenceType.Single:
                    await HandleSingle(userId, request, transactionType);
                    break;

                case OccurrenceType.Installment:
                    await HandleInstallment(userId, request, transactionType);
                    break;

                case OccurrenceType.Recurring:
                    await HandleRecurring(userId, request, transactionType);
                    break;

                default:
                    throw new InvalidOperationException("Invalid occurrence type.");
            }
        }

        private static TransactionType MapEntryType(EntryType type) {
            return type == EntryType.Income
                ? TransactionType.Income
                : TransactionType.Expense;
        }

        private static bool IsTransfer(CreateTransactionRequest request) {
            return request.FromAccountId.HasValue && request.ToAccountId.HasValue;
        }

        private async Task HandleSingle(
            Guid userId,
            CreateTransactionRequest request,
            TransactionType type) {
            var transaction = new Transaction(
                userId,
                request.AccountId!.Value,
                request.Value,
                type,
                request.StartDate,
                request.Description,
                request.CategoryId,
                OccurrenceType.Single,
                null,
                null,
                null
            );

            await _repository.AddAsync(transaction);
        }

        private async Task HandleInstallment(
            Guid userId,
            CreateTransactionRequest request,
            TransactionType type) {
            if(!request.InstallmentFrom.HasValue || !request.InstallmentTo.HasValue)
                throw new InvalidOperationException("Installment range required.");

            var groupId = Guid.NewGuid();
            var transactions = new List<Transaction>();

            for(int i = request.InstallmentFrom.Value; i <= request.InstallmentTo.Value; i++) {
                var date = request.StartDate.AddMonths(i - 1);

                transactions.Add(new Transaction(
                    userId,
                    request.AccountId!.Value,
                    request.Value,
                    type,
                    date,
                    $"{request.Description} ({i}/{request.InstallmentTo})",
                    request.CategoryId,
                    OccurrenceType.Installment,
                    groupId,
                    i,
                    request.InstallmentTo
                ));
            }

            await _repository.AddRangeAsync(transactions);
        }

        private async Task HandleRecurring(
            Guid userId,
            CreateTransactionRequest request,
            TransactionType type) {

            var groupId = Guid.NewGuid();
            var endDate = request.EndDate ?? request.StartDate.AddYears(80);

            var transactions = new List<Transaction>();
            var currentDate = request.StartDate;

            while(currentDate <= endDate) {
                transactions.Add(new Transaction(
                    userId,
                    request.AccountId!.Value,
                    request.Value,
                    type,
                    currentDate,
                    request.Description,
                    request.CategoryId,
                    OccurrenceType.Recurring,
                    groupId,
                    null,
                    null
                ));

                currentDate = AddRecurrence(currentDate, request.Recurrence);
            }

            await _repository.AddRangeAsync(transactions);
        }

        private async Task HandleTransfer(
            Guid userId,
            CreateTransactionRequest request,
            TransactionType type) {
            var groupId = Guid.NewGuid();
            var transactions = new List<Transaction>();

            transactions.Add(new Transaction(
                userId,
                request.FromAccountId!.Value,
                request.Value,
                TransactionType.Expense,
                request.StartDate,
                request.Description,
                null,
                OccurrenceType.Single,
                groupId,
                null,
                null
            ));

            transactions.Add(new Transaction(
                userId,
                request.ToAccountId!.Value,
                request.Value,
                TransactionType.Income,
                request.StartDate,
                request.Description,
                null,
                OccurrenceType.Single,
                groupId,
                null,
                null
            ));

            await _repository.AddRangeAsync(transactions);
        }

        private static DateTime AddRecurrence(DateTime date, RecurrenceType recurrence) {
            return recurrence switch {
                RecurrenceType.Weekly => date.AddDays(7),
                RecurrenceType.BiWeekly => date.AddDays(14),
                RecurrenceType.Monthly => date.AddMonths(1),
                RecurrenceType.Yearly => date.AddYears(1),
                _ => throw new InvalidOperationException("Invalid recurrence.")
            };
        }
    }
}