using ControleFinanceiro.Application.Common.Exceptions;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.ValueObjetcs;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Auth {
    public class ChangePasswordUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly PasswordHasher<object> _verifier = new();

        public ChangePasswordUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher) {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task ExecuteAsync(Guid userId, string currentPassword, string newPassword) {
            var user = await _userRepository.GetByIdAsync(userId);

            if(user == null)
                throw new BusinessException("Usuário inválido.");

            var result = _verifier.VerifyHashedPassword(
                null!,
                user.PasswordHash.Value,
                currentPassword);

            if(result == PasswordVerificationResult.Failed)
                throw new BusinessException("Senha atual incorreta.");

            user.ChangePassword(new PasswordHash(
                _passwordHasher.Hash(newPassword)));

            await _userRepository.UpdateAsync(user);
        }
    }
}
