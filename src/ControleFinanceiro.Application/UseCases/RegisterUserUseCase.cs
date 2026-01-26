using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.ValueObjetcs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases {
    public class RegisterUserUseCase {

        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher) {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
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

            return new RegisterUserResponse {
                UserId = user.Id,
                Email = user.Email.Value
            };
        }
    }
}
