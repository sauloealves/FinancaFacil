using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases.Accounts;
using ControleFinanceiro.Domain.Entities;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Tests.Application.Accounts {
    public class CreateAccountUseCaseTests {
        [Fact]
        public async Task Should_Create_Account_Successfully() {
            var repositoryMock = new Mock<IAccountRepository>();

            var useCase = new CreateAccountUseCase(repositoryMock.Object);

            var userId = Guid.NewGuid();

            var request = new CreateAccountRequest {
                Name = "Conta Corrente",
                InitialBalance = 1000
            };

            await useCase.AddAsync(userId, request.Name,request.InitialBalance);

            repositoryMock.Verify(r =>
                r.AddAsync(It.Is<Account>(a =>
                    a.Name == request.Name &&
                    a.InitialBalance == request.InitialBalance &&
                    a.UserId == userId
                )),
                Times.Once);
        }
    }
}
