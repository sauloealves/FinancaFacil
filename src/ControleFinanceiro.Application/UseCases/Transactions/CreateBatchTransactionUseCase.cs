using ControleFinanceiro.Application.Builder;
using ControleFinanceiro.Application.DTOs.Transaction;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Transactions {
    public class CreateBatchTransactionUseCase {
        private readonly ITransactionRepository _transactionRepository;
        
        public CreateBatchTransactionUseCase(ITransactionRepository transactionRepository) {
            _transactionRepository = transactionRepository;
        }

        public async Task ExecuteAsync(Guid userId, List<CreateTransactionRequest> requests) {
            var transactions = new List<Transaction>();

            foreach(var request in requests) {
                var builder = new TransactionBuilder(userId, request);
                transactions.AddRange(builder.Build());
            }

            await _transactionRepository.AddRangeAsync(transactions);
        }
    }
}
