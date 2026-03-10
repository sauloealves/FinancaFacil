using ControleFinanceiro.Application.Interfaces;

public class DeleteAccountUseCase {
    private readonly IAccountRepository _accountRepository;

    public DeleteAccountUseCase(IAccountRepository accountRepository) {
        _accountRepository = accountRepository;
    }

    public async Task DeleteAsync(Guid accountId, Guid userId) {
        
        var account = await _accountRepository.GetByIdAsync(accountId, userId);

        if (account == null) {
            throw new Exception("Account not found or access denied.");
        }
        await _accountRepository.DeleteAsync(account);
    }
}