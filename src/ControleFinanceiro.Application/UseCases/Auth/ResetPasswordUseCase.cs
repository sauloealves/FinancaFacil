using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.ValueObjetcs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Auth {
    public class ResetPasswordUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPasswordUseCase(
            IUserRepository userRepository,
            IPasswordResetTokenRepository tokenRepository,
            IPasswordHasher passwordHasher) {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task ExecuteAsync(ResetPasswordRequest request) {
            var token = await _tokenRepository.GetValidAsync(request.Token);
            if(token == null)
                throw new InvalidOperationException("Token inválido.");

            var user = await _userRepository.GetByIdAsync(token.UserId);

            if(user == null)
                throw new InvalidOperationException("Usuário inválido.");

            user.ChangePassword(new PasswordHash(
                _passwordHasher.Hash(request.NewPassword)));

            token.MarkAsUsed();

            await _tokenRepository.UpdateAsync(token);
            await _userRepository.UpdateAsync(user);
        }
    }
}
