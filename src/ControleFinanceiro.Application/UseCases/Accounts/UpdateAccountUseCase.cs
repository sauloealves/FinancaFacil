using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.UseCases.Accounts {
    public class UpdateAccountUseCase {

        private readonly IAccountRepository _repository;

        public UpdateAccountUseCase(IAccountRepository accountRepository) {
            _repository = accountRepository;
        }

        public async Task UpdateAsync(Guid accountId, Guid userId, string name, decimal initialBalance, bool isEnabled) {
            
            var account = await _repository.GetByIdAsync(accountId, userId);
            
            if (account == null) {
                throw new Exception("Account not found or access denied.");
            }

            account.Update(name, initialBalance, isEnabled);
            await _repository.UpdateAsync(account);
        }
    }
}
