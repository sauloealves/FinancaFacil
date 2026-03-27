using ControleFinanceiro.Application.DTOs.Account;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Application.UseCases.Transactions {
    public class GetBalanceUseCase {
        private readonly ITransactionRepository _repository;
        private readonly IAccountRepository _accountRepository;

        public GetBalanceUseCase(ITransactionRepository repository, IAccountRepository accountRepository) {
            _repository = repository;
            _accountRepository = accountRepository;
        }

        public async Task<BalanceResponse> ExecuteAsync(Guid userId, GetBalanceRequest request) {

                var referenceDate = new DateTime(request.Year, request.Month, request.Day).AddDays(-1);

                if (request.AccountId.HasValue) {
                    var account = await _accountRepository.GetByIdAsync(request.AccountId.Value, userId);

                    if (account == null) {
                        throw new Exception("Account not found");                
                    }
                }

            var sumInitialBalance = await _accountRepository.GetSumInitialBalanceAsync(userId, request.AccountId);
            var balance = await _repository.GetBalanceAsync(request.AccountId, userId, referenceDate) + sumInitialBalance;

            return new BalanceResponse() { Balance = balance, ReferenceDate = referenceDate};
        }
    }
}
