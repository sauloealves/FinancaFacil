using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases {
    public class AccountUseCase {

        private readonly IAccountRepository _repository;

        public AccountUseCase(IAccountRepository accountRepository) {
            _repository = accountRepository;
        }

        public async Task AddAsync(Guid userId, string name, decimal initialBalance) {
            var account = new Account(userId, name, initialBalance);
            await _repository.AddAsync(account);
        }
    }
}
