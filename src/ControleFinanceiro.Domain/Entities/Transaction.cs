using ControleFinanceiro.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Entities {
    public class Transaction {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; }

        public Guid AccountId { get; private set; }

        public Guid? CategoryId { get; private set; }

        public decimal Amount { get; private set; }

        public TransactionType Type { get; private set; }

        public DateTime Date { get; private set; }

        public Guid? TransferGroupId { get; private set; }

        public string Description { get; private set; }

        public bool IsDeleted { get; private set; }

        private Transaction() { }

        public Transaction(Guid userId,Guid accountId,decimal amount,TransactionType type,DateTime date,string description,Guid?categoryId = null,Guid? transferGroupId = null) {
            Id = Guid.NewGuid();
            UserId = userId;
            AccountId = accountId;
            Amount = amount;
            Type = type;
            Date = date;
            Description = description;
            CategoryId = categoryId;
            TransferGroupId = transferGroupId;
            IsDeleted = false;
        }

        public void Delete() {
            IsDeleted = true;
        }
    }
}
