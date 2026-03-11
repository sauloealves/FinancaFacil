using ControleFinanceiro.Application.Constants;
using ControleFinanceiro.Application.DTOs.Auth;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.ValueObjetcs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Auth {
    public class RegisterUserUseCase {

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAccountRepository _accountRepository;
        private readonly ICategoryRepository _categoryRepository;

        public RegisterUserUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ICategoryRepository categoryRepository,
            IAccountRepository accountRepository) {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
        }
        public async Task<RegisterUserResponse> ExecuteAsync(RegisterUserRequest request) {
            // Validação básica
            if(string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Nome é obrigatório.");

            if(string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("E-mail é obrigatório.");

            if(string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Senha é obrigatória.");

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if(existingUser != null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            var email = new Email(request.Email);
            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(
                request.Name,
                email,
                new PasswordHash(passwordHash)
            );

            await _userRepository.AddAsync(user);

            var accounts = DefaultData.Accounts
                .Select(name => new Account(user.Id, name, 0))
                .ToList();

            await _accountRepository.AddRangeAsync(accounts);

                var categories = DefaultData.Categories
                    .Select(name => new Category(user.Id, name))
                    .ToList();

            await _categoryRepository.AddRangeAsync(categories);

            return new RegisterUserResponse {
                UserId = user.Id,
                Email = user.Email.Value
            };
        }
    }
}
