using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases {
    public class CreateTransactionUseCase {
        private readonly ITransactionRepository _repository;

        public CreateTransactionUseCase(ITransactionRepository repository) {
            _repository = repository;
        }

        public async Task AddAsync(Guid userId, CreateTransactionRequest request) {
            var type = (TransactionType)request.Type;

            if(type == TransactionType.Transferencia) {
                if(!request.DestinationAccountId.HasValue)
                    throw new InvalidOperationException("Conta destino obrigatória.");

                var groupId = Guid.NewGuid();

                var debit = new Transaction(
                    userId,
                    request.AccountId,
                    request.Amount,
                    TransactionType.Despesa,
                    request.Date,
                    request.Description,
                    null,
                    groupId
                );

                var credit = new Transaction(
                    userId,
                    request.DestinationAccountId.Value,
                    request.Amount,
                    TransactionType.Receita,
                    request.Date,
                    request.Description,
                    null,
                    groupId
                );

                await _repository.AddRangeAsync(new List<Transaction> { debit, credit });
            } else {
                if(!request.CategoryId.HasValue)
                    throw new InvalidOperationException("Categoria obrigatória.");

                var transaction = new Transaction(
                    userId,
                    request.AccountId,
                    request.Amount,
                    type,
                    request.Date,
                    request.Description,
                    request.CategoryId
                );

                await _repository.AddAsync(transaction);
            }
        }
    }
}
