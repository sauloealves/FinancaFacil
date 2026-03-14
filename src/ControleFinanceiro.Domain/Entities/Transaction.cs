
using ControleFinanceiro.Domain.Enums;

using static ControleFinanceiro.Domain.Enums.OccurrenceType;

namespace ControleFinanceiro.Domain.Entities {
    public class Transaction {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }

        public Guid AccountId { get; private set; }
        public Guid? CategoryId { get; private set; }

        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public DateTime Date { get; private set; }

        public OccurrenceType OccurrenceType { get; private set; }
        public Guid? OccurrenceGroupId { get; private set; }

        public int? InstallmentNumber { get; private set; }
        public int? InstallmentTotal { get; private set; }

        public string Description { get; private set; }

        public bool IsDeleted { get; private set; }

        private Transaction() { }

        public Transaction(
            Guid userId,
            Guid accountId,
            decimal amount,
            TransactionType type,
            DateTime date,
            string description,
            Guid? categoryId,
            OccurrenceType occurrenceType,
            Guid? occurrenceGroupId,
            int? installmentNumber,
            int? installmentTotal) {
            Id = Guid.NewGuid();
            UserId = userId;
            AccountId = accountId;
            Amount = amount;
            Type = type;
            Date = date;
            Description = description;
            CategoryId = categoryId;
            OccurrenceType = occurrenceType;
            OccurrenceGroupId = occurrenceGroupId;
            InstallmentNumber = installmentNumber;
            InstallmentTotal = installmentTotal;
            IsDeleted = false;
        }

        public void Delete() {
            IsDeleted = true;
        }

        public void Update(
        string description,
        decimal amount,
        Guid? categoryId,
        DateTime date,
        Guid accountId) {
                Description = description;
                Amount = amount;
                CategoryId = categoryId;
                Date = date;
                AccountId = accountId;
        }

    }
}