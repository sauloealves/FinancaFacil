// ControleFinanceiro.Tests/Application/Accounts/GetAccountsUseCaseTests.cs
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases;
using ControleFinanceiro.Application.UseCases.Accounts;
using ControleFinanceiro.Domain.Entities;

using Moq;

namespace ControleFinanceiro.Tests.Application.Accounts {
    public class GetAccountsUseCaseTests {
        [Fact]
        public async Task Should_Return_Accounts_For_User() {
            var userId = Guid.NewGuid();

            var accounts = new List<Account>
            {
                new Account(userId, "Conta A", 100),
                new Account(userId, "Conta B", 200)
            };

            var repositoryMock = new Mock<IAccountRepository>();
            var transactionMock = new Mock<ITransactionRepository>();
            repositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                          .ReturnsAsync(accounts);

            var useCase = new GetAccountsUseCase(repositoryMock.Object, transactionMock.Object);

            var result = await useCase.GetByUserId(userId);

            Assert.Equal(2, result.Count());
            Assert.Equal("Conta A", result.First().Name);
            Assert.Equal(100, result.First().InitialBalance);
        }

        [Fact]
        public async Task Should_Return_Accounts_With_Correct_CurrentBalance() {
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
            accountRepoMock.Setup(r => r.GetByUserIdAsync(userId))
                           .ReturnsAsync(accounts);

            var transactionRepoMock = new Mock<ITransactionRepository>();
            transactionRepoMock.Setup(r =>
                r.GetAccountBalanceAsync(accountId, userId, It.IsAny<DateTime>()))
                .ReturnsAsync(50);

            var useCase = new GetAccountsUseCase(
                accountRepoMock.Object,
                transactionRepoMock.Object);

            var result = await useCase.GetByUserId(userId);

            Assert.Single(result);
            Assert.Equal(150, result.First().CurrentBalance);
        }


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
            accountRepoMock.Setup(r => r.GetByUserIdAsync(userId))
                           .ReturnsAsync(accounts);

            var transactionRepoMock = new Mock<ITransactionRepository>();
            transactionRepoMock.Setup(r =>
                r.GetAccountBalanceAsync(
                    accountId,
                    userId,
                    It.IsAny<DateTime>()))
                .ReturnsAsync(50);

            var useCase = new GetAccountsUseCase(
                accountRepoMock.Object,
                transactionRepoMock.Object);

            var result = await useCase.GetByUserId(userId);

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
