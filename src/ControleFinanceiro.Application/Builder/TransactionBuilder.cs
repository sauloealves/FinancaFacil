using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Builder {
    public class TransactionBuilder {
        private readonly Guid _userId;
        private readonly CreateTransactionRequest _request;
        public TransactionBuilder(Guid userId, CreateTransactionRequest request) {
            _userId = userId;
            _request = request;
        }

        public List<Transaction> Build() {
            var type = MapEntryType(_request.Type);

            if(IsTransfer())
                return BuildTransfer();

            return _request.OccurrenceType switch {
                OccurrenceType.Single => BuildSingle(type),
                //OccurrenceType.Installment => BuildInstallment(type),
                //OccurrenceType.Recurring => BuildRecurring(type),
                _ => throw new InvalidOperationException("Invalid occurrence type.")
            };
        }
        private List<Transaction> BuildSingle(TransactionType type) {
            return new List<Transaction> {
                new Transaction(
                    _userId,
                    _request.AccountId!.Value,
                    _request.Value,
                    type,
                    _request.StartDate,
                    _request.Description,
                    _request.CategoryId,
                    OccurrenceType.Single,
                    null,
                    null,
                    null
                )
            };
        }
        private List<Transaction> BuildTransfer() {
            var groupId = Guid.NewGuid();

            return new List<Transaction> {
                new Transaction(
                    _userId,
                    _request.FromAccountId!.Value,
                    _request.Value,
                    TransactionType.Expense,
                    _request.StartDate,
                    _request.Description,
                    null,
                    OccurrenceType.Single,
                    groupId,
                    null,
                    null
                ),
                new Transaction(
                    _userId,
                    _request.ToAccountId!.Value,
                    _request.Value,
                    TransactionType.Income,
                    _request.StartDate,
                    _request.Description,
                    null,
                    OccurrenceType.Single,
                    groupId,
                    null,
                    null
                )
            };
        }

        private static TransactionType MapEntryType(EntryType type) {
            return type == EntryType.Income
                ? TransactionType.Income
                : TransactionType.Expense;
        }
        private bool IsTransfer() {
            return _request.FromAccountId.HasValue && _request.ToAccountId.HasValue;
        }
    }

}
