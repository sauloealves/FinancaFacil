using ControleFinanceiro.Application.DTOs.Auth;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Auth {
    public class ForgotPasswordUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ForgotPasswordUseCase(
            IUserRepository userRepository,
            IPasswordResetTokenRepository tokenRepository,
            IEmailService emailService,
            IConfiguration configuration
            ) {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task ExecuteAsync(ForgotPasswordRequest request) {
            
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if(user == null)
                return;

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            var reset = new PasswordResetToken(
                user.Id, token, DateTime.Now.AddDays(1));

            await _tokenRepository.AddAsync(reset);

            var link = $"{_configuration["Account:UrlResetSenha"]}/reset-password?token={token}";

            await _emailService.SendAsync(
                user.Email.Value,
                "Recuperação de senha",
                $"Acesse o link para redefinir sua senha: {link}"
            );
        }
    }
}
