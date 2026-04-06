using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;

using System.Text.RegularExpressions;

namespace ControleFinanceiro.Application.UseCases.Transactions {
    public class UpdateTransactionUseCase {
        private readonly ITransactionRepository _repository;
        private readonly CreateTransactionUseCase _createUseCase;

        public UpdateTransactionUseCase(
            ITransactionRepository repository,
            CreateTransactionUseCase createUseCase) {
            _repository = repository;
            _createUseCase = createUseCase;
        }

        public async Task ExecuteAsync(Guid transactionId, Guid userId, UpdateTransactionRequest request) {
            var existing = await _repository
                .GetByIdAsync(transactionId, userId);

            if (existing == null)
                throw new InvalidOperationException("Transaction not found.");

            var isStructuralChange = IsStructuralChange(existing, request);

            if (!existing.OccurrenceGroupId.HasValue) {
                await HandleSingleUpdate(userId, existing, request, isStructuralChange);
                return;
            }

            await HandleGroupedUpdate(userId, existing, request, isStructuralChange);
        }

        private static bool IsStructuralChange(Transaction existing, UpdateTransactionRequest request) {
            if (existing.Type.ToString() != request.Type.ToString())
                return true;

            if (existing.OccurrenceType.ToString() != request.OccurrenceType.ToString())
                return true;

            if (request.FromAccountId.HasValue && request.ToAccountId.HasValue)
                return true;

            return false;
        }

        private async Task HandleSingleUpdate(
            Guid userId,
            Transaction existing,
            UpdateTransactionRequest request,
            bool structuralChange) {
            if (structuralChange) {
                if (existing.OccurrenceGroupId != null) {
                    var group = await _repository.GetByGroupAsync(existing.OccurrenceGroupId.Value, userId);
                    if (group.Count == 2 &&
                        group.Any(t => t.Type == TransactionType.Income) &&
                        group.Any(t => t.Type == TransactionType.Expense) &&
                        group.All(t => t.CategoryId == null)) {
                        foreach (var t in group)
                            t.Delete();
                    } else {
                        existing.Delete();
                    }
                } else {
                    existing.Delete();
                }

                await _repository.SaveChangesAsync();
                await _createUseCase.AddAsync(userId, MapToCreate(request));
                return;
            }

            existing.Update(
                request.Description,
                request.Value,
                request.CategoryId,
                request.StartDate,
                request.AccountId,
                request.SendNotification ?? false
            );

            await _repository.SaveChangesAsync();
        }

        private async Task HandleGroupedUpdate(
            Guid userId,
            Transaction existing,
            UpdateTransactionRequest request,
            bool structuralChange) {
            var groupId = existing.OccurrenceGroupId!.Value;
            var group = await _repository.GetByGroupAsync(groupId, userId);

            if (request.EditMode == "single") {
                if (structuralChange) {
                    existing.Delete();
                    await _repository.SaveChangesAsync();

                    var createRequest = MapToCreate(request);
                    createRequest.OccurrenceType = OccurrenceType.Single;
                    createRequest.InstallmentFrom = null;
                    createRequest.InstallmentTo = null;                    
                    createRequest.EndDate = null;

                    await _createUseCase.AddAsync(userId, createRequest);
                } else {
                    existing.Update(
                        request.Description,
                        request.Value,
                        request.CategoryId,
                        request.StartDate,
                        request.AccountId,
                        request.SendNotification ?? false
                    );

                    await _repository.SaveChangesAsync();
                }
                return;
            }

            if (request.EditMode == "fromBeginning") {
                await UpdateGroupFromBeginning(userId, group, request, existing);
                return;
            }

            if (request.EditMode == "fromThis") {
                await UpdateGroupFromThis(userId, group, existing, request);
            }
        }

        private async Task UpdateGroupFromBeginning(
            Guid userId,
            List<Transaction> group,
            UpdateTransactionRequest request,
            Transaction referenceTransaction) {
            var transactionType = request.Type == EntryType.Income
                ? TransactionType.Income
                : TransactionType.Expense;

            if (referenceTransaction.OccurrenceType == OccurrenceType.Installment) {
                await UpdateInstallmentGroup(group, request, transactionType, referenceTransaction.InstallmentNumber ?? 1, group.Count);
            } else if (referenceTransaction.OccurrenceType == OccurrenceType.Recurring) {
                await UpdateRecurringGroup(userId, group, request, transactionType, group.First().Date);
            }

            await _repository.SaveChangesAsync();
        }

        private async Task UpdateGroupFromThis(
            Guid userId,
            List<Transaction> group,
            Transaction existing,
            UpdateTransactionRequest request) {
            var transactionType = request.Type == EntryType.Income
                ? TransactionType.Income
                : TransactionType.Expense;

            var itemsToUpdate = group.Where(t => t.Date >= existing.Date).OrderBy(t => t.Date).ToList();

            if (existing.OccurrenceType == OccurrenceType.Installment) {
                await UpdateInstallmentGroup(itemsToUpdate, request, transactionType, existing.InstallmentNumber ?? 1, group.Count);
            } else if (existing.OccurrenceType == OccurrenceType.Recurring) {
                await UpdateRecurringGroup(userId, itemsToUpdate, request, transactionType, existing.Date);
            }

            await _repository.SaveChangesAsync();
        }

        private async Task UpdateInstallmentGroup(List<Transaction> transactions,UpdateTransactionRequest request,TransactionType type,int installmentNumber,int originalCountGroup) {
            
            int contAddMonths = 0;
            var orderedTransactions = transactions.OrderBy(t => t.Date).ToList();
            var totalInstallments = orderedTransactions.Count;
            
            if (request.EditMode == "fromBeginning")
                contAddMonths =  (installmentNumber - 1) * -1;
            else if (request.EditMode == "fromThis")
                totalInstallments = originalCountGroup;


            for (int i = 0; i < Math.Min(orderedTransactions.Count, totalInstallments); i++) {
                var transaction = orderedTransactions[i];                                
                var acrescimento = request.EditMode == "fromThis" ? installmentNumber + i : i + 1;

                transaction.Update(
                    $"{Regex.Replace(request.Description, @"\s*\(\d+\/\d+\)$", "")} ({ acrescimento }/{totalInstallments})",
                    request.Value,
                    request.CategoryId,
                    request.StartDate.AddMonths(contAddMonths),
                    request.AccountId,
                    request.SendNotification ?? false
                );

                contAddMonths++;
            }            
        }

        private async Task UpdateRecurringGroup(
            Guid userId,
            List<Transaction> transactions,
            UpdateTransactionRequest request,
            TransactionType type,
            DateTime startDate) {
            if (request.Recurrence == null)
                throw new InvalidOperationException("Recurrence type required.");

            var endDate = request.EndDate ?? startDate.AddYears(80);
            var orderedTransactions = transactions.OrderBy(t => t.Date).ToList();

            // Calcular quantas transações são necessárias
            var expectedDates = new List<DateTime>();
            var currentDate = startDate;
            while (currentDate <= endDate) {
                expectedDates.Add(currentDate);
                currentDate = AddRecurrence(currentDate, request.Recurrence);
            }

            // Atualizar transações existentes
            for (int i = 0; i < Math.Min(orderedTransactions.Count, expectedDates.Count); i++) {
                var transaction = orderedTransactions[i];
                transaction.Update(
                    request.Description,
                    request.Value,
                    request.CategoryId,
                    expectedDates[i],
                    request.AccountId,
                    request.SendNotification ?? false
                );
            }           
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

        private static CreateTransactionRequest MapToCreate(UpdateTransactionRequest request) {
            return new CreateTransactionRequest {
                Description = request.Description,
                Value = request.Value,
                CategoryId = request.CategoryId,
                AccountId = request.AccountId,
                StartDate = request.StartDate,
                OccurrenceType = request.OccurrenceType,
                InstallmentFrom = request.InstallmentFrom,
                InstallmentTo = request.InstallmentTo,
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Recurrence = request.Recurrence,
                EndDate = request.EndDate,
                Type = request.Type
            };
        }
    }
}
