// ControleFinanceiro.Tests/Application/Accounts/GetAccountsUseCaseTests.cs
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases;
using ControleFinanceiro.Application.UseCases.Accounts;
using ControleFinanceiro.Domain.Entities;

using Moq;

namespace ControleFinanceiro.Tests.Application.Accounts {
    public class GetAccountsUseCaseTests {        
        [Fact]
        public async Task Should_Use_Current_Date_As_Reference_For_Balance() {
            var userId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var accounts = new List<Account>
            {
                new Account(userId, "Conta A", 100)
            };

            typeof(Account)
                .GetProperty("Id")!
                .SetValue(accounts[0], accountId);

            var accountRepoMock = new Mock<IAccountRepository>();
            accountRepoMock.Verify(r => r.GetByUserIdAsync(userId),Times.Once);
            accountRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(accounts);

            var transactionRepoMock = new Mock<ITransactionRepository>();

            transactionRepoMock.Setup(r => r.GetAccountBalanceAsync(accountId,userId,It.IsAny<DateTime>())).ReturnsAsync(50);

            var useCase = new GetAccountsUseCase(
                accountRepoMock.Object);

            var result = await useCase.GetByUserIdAsync(userId);

            Assert.Equal(150, result.First().CurrentBalance);

            transactionRepoMock.Verify(r =>
                r.GetAccountBalanceAsync(
                    accountId,
                    userId,
                    It.Is<DateTime>(d => d <= DateTime.UtcNow)),
                Times.Once);
        }

    }
}
