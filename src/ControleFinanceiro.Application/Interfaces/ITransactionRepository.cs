using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Interfaces {
    public interface ITransactionRepository {
        Task AddAsync(Transaction transaction);
        Task AddRangeAsync(List<Transaction> transactions);
    }
}
