using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Accounts {
    public class GetAccountsUseCase {

        private readonly IAccountRepository _repository;
        private readonly ITransactionRepository _transactionRepository;
        public GetAccountsUseCase(IAccountRepository accountRepository, ITransactionRepository transactionRepository) {
            _repository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IEnumerable<AccountResponseDTO>> GetByUserId(Guid userId) {

            var accounts = await _repository.GetByUserIdAsync(userId);

            var result = new List<AccountResponseDTO>();

            foreach(var account in accounts) {
                var balance = await _transactionRepository.GetAccountBalanceAsync(account.Id, userId, DateTime.UtcNow);

                result.Add(new AccountResponseDTO {
                    Id = account.Id,
                    Name = account.Name,
                    InitialBalance = account.InitialBalance,
                    CurrentBalance = account.InitialBalance + balance
                });
            }

            return result.AsEnumerable();        

        }
    }
}
