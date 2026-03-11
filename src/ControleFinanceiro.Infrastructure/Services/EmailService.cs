using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Services {
    public class EmailService :IEmailService {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger) {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task SendAsync(string to, string subject, string body) {
            try {
                var smtp = _configuration.GetSection("Smtp");

                var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]!)) {
                    Credentials = new NetworkCredential(
                        smtp["Username"],
                        smtp["Password"]
                    ),
                    EnableSsl = true
                };

                _logger.LogInformation("SMTP Host: {Host}", smtp["Host"]);
                _logger.LogInformation("SMTP Port: {Port}", smtp["Port"]);
                _logger.LogInformation("SMTP Username: {User}", smtp["Username"]);
                _logger.LogInformation("SMTP From: {From}", smtp["From"]);

                var mail = new MailMessage {
                    From = new MailAddress(smtp["From"]!),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(to);
                _logger.LogInformation("Email enviado para {To}", to);

                await client.SendMailAsync(mail);

            } catch(Exception ex) {

                _logger.LogError(ex, "Erro ao enviar email");
                throw;
            }
        }
    }
}
