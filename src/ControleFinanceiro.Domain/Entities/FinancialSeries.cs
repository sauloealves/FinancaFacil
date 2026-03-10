using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Entities {
    public class FinancialSeries {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Description { get; private set; }

        public string OccurrenceType { get; private set; }
        public string? Recurrence { get; private set; }

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public Guid? AccountId { get; private set; }
        public Guid? CategoryId { get; private set; }

        public Guid? FromAccountId { get; private set; }
        public Guid? ToAccountId { get; private set; }

        private FinancialSeries() { }

        public FinancialSeries(
            Guid userId,
            string description,
            string occurrenceType,
            string? recurrence,
            DateTime startDate,
            DateTime endDate,
            Guid? accountId,
            Guid? categoryId,
            Guid? fromAccountId,
            Guid? toAccountId) {
            Id = Guid.NewGuid();
            UserId = userId;
            Description = description;
            OccurrenceType = occurrenceType;
            Recurrence = recurrence;
            StartDate = startDate;
            EndDate = endDate;
            AccountId = accountId;
            CategoryId = categoryId;
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
        }
    }
}
