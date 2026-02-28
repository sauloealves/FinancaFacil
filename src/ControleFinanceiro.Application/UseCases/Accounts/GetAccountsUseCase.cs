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
        public GetAccountsUseCase(IAccountRepository accountRepository) {
            _repository = accountRepository;            
        }

        public async Task<IEnumerable<AccountResponseDTO>> GetByUserIdAsync(Guid userId) {

            var result = new List<AccountResponseDTO>();

            var balance = await _repository.GetAccountWithBalanceAsync(userId, DateTime.UtcNow);                

            balance.ToList().ForEach(account => {
                var accountResponse = new AccountResponseDTO {
                    Id = account.Id,
                    Name = account.Name,
                    InitialBalance = account.InitialBalance,
                    CurrentBalance = account.CurrentBalance
                };
                result.Add(accountResponse);
            });

            return result.AsEnumerable();        

        }

        public async Task DeleteAsync(Guid accountId, Guid userId) {
            var account = await _repository.GetByIdAsync(accountId, userId);
            if (account == null) {
                throw new Exception("Account not found");
            }
            await _repository.DeleteAsync(account);
        }
    }
}
