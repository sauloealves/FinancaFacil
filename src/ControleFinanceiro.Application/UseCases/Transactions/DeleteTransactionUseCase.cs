using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Transactions {

    public class DeleteTransactionUseCase {

        private readonly ITransactionRepository _transactionRepository;

        public DeleteTransactionUseCase(ITransactionRepository transactionRepository) {
            _transactionRepository = transactionRepository;
        }
        public async Task ExecuteAsync(Guid transactionId, Guid userId, DeleteScope scope) {
            
            var transaction = await _transactionRepository.GetByIdAsync(transactionId,userId);

            if (transaction == null || transaction.UserId != userId)
                throw new InvalidOperationException("Transação não encontrada ou acesso negado.");

            if(transaction.OccurrenceGroupId == null) {
                transaction.Delete();
                await _transactionRepository.SaveChangesAsync();
                return;
            }

            var group = await _transactionRepository
                .GetByGroupAsync(transaction.OccurrenceGroupId.Value, userId);

            switch(scope) {
                case DeleteScope.OnlyThis:
                    transaction.Delete();
                    break;

                case DeleteScope.FromThis:
                    foreach(var item in group
                        .Where(t => t.Date >= transaction.Date)) {
                        item.Delete();
                    }
                    break;

                case DeleteScope.FromFirst:
                    foreach(var item in group) {
                        item.Delete();
                    }
                    break;
            }

            await _transactionRepository.SaveChangesAsync();
        }
    }
}
