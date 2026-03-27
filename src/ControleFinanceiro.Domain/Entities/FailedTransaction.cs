using ControleFinanceiro.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Entities {
    public class FailedTransaction {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }

        public string OriginalText { get; private set; }

        public TransactionType? SuggestedType { get; private set; }
        public decimal? SuggestedAmount { get; private set; }
        public DateTime? SuggestedDate { get; private set; }
        public Guid? SuggestedAccount { get; private set; }
        public Guid? SuggestedCategory { get; private set; }

        public string Reason { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public bool IsResolved { get; set; }

        public FailedTransaction(Guid userId, string originalText, string reason, TransactionType? suggestedType = null, decimal? suggestedAmount = null, DateTime? suggestedDate = null, Guid? suggestedAccount = null, Guid? suggestedCategory = null) {
            Id = Guid.NewGuid();
            UserId = userId;
            OriginalText = originalText;
            Reason = reason;
            SuggestedType = suggestedType;
            SuggestedAmount = suggestedAmount;
            SuggestedDate = suggestedDate;
            SuggestedAccount = suggestedAccount;
            SuggestedCategory = suggestedCategory;
            CreatedAt = DateTime.Now;
        }

        public void Resolve(Guid resolvedTransactionId) {
            IsResolved = true;
        }
    }
}
