using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases.Categories;
using ControleFinanceiro.Domain.Entities;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Tests.Application.Categories {
    public class CreateCatetoryUseCase {
        [Fact]
        public async Task Should_Create_Category_Successfully() {
            var repositoryMock = new Mock<ICategoryRepository>();
            var useCase = new CreateCategoryUseCase(repositoryMock.Object);
            var userId = Guid.NewGuid();
            var request = new CreateCategoryRequest {
                Name = "Alimentação"
            };
            await useCase.AddAsync(userId, request.Name);
            repositoryMock.Verify(r =>
                r.AddAsync(It.Is<Category>(c =>
                    c.Name == request.Name &&
                    c.UserId == userId
                )),
                Times.Once);
        }
    }
}
