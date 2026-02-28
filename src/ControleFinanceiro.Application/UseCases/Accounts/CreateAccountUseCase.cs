using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.UseCases.Accounts {
    public class CreateAccountUseCase {


        private readonly IAccountRepository _repository;

        public CreateAccountUseCase(IAccountRepository accountRepository) {
            _repository = accountRepository;
        }

        public async Task AddAsync(Guid userId, string name, decimal initialBalance) {
            var account = new Account(userId, name, initialBalance);
            await _repository.AddAsync(account);
        }        
    }
}
