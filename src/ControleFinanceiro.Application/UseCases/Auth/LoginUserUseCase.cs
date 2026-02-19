using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Auth {
    public class LoginUserUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<object> _verifier = new();

        public LoginUserUseCase(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService) {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> ExecuteAsync(LoginRequest request) {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if(user == null || !user.Active)
                throw new InvalidOperationException("Usuário ou senha inválidos.");

            var result = _verifier.VerifyHashedPassword(
                null!,
                user.PasswordHash.Value,
                request.Password);

            if(result == PasswordVerificationResult.Failed)
                throw new InvalidOperationException("Usuário ou senha inválidos.");

            var token = _tokenService.GenerateToken(user, out var expiration);

            return new LoginResponse {
                Token = token,
                Expiration = expiration
            };
        }
    }
}
