using ControleFinanceiro.Application.Common.Exceptions;
using ControleFinanceiro.Application.Interfaces;

public class DeleteAccountUseCase {
    private readonly IAccountRepository _accountRepository;

    public DeleteAccountUseCase(IAccountRepository accountRepository) {
        _accountRepository = accountRepository;
    }

    public async Task DeleteAsync(Guid accountId, Guid userId) {
        
        var account = await _accountRepository.GetByIdAsync(accountId, userId);

        if (account == null) {
            throw new BusinessException("Account not found or access denied.");
        }

        if(await _accountRepository.HasTransactions(accountId, userId))
            throw new BusinessException("Não é possível excluir uma conta que possui transações.");

        await _accountRepository.DeleteAsync(account);
    }
}