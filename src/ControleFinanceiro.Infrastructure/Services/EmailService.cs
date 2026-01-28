using ControleFinanceiro.Application.Interfaces;

using Microsoft.Extensions.Configuration;

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
        public EmailService(IConfiguration configuration) {
            _configuration = configuration;
        }
        public async Task SendAsync(string to, string subject, string body) {
            var smtp = _configuration.GetSection("Smtp");

            var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]!)) {
                Credentials = new NetworkCredential(
                    smtp["Username"],
                    smtp["Password"]
                ),
                EnableSsl = true
            };

            var mail = new MailMessage {
                From = new MailAddress(smtp["From"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }
    }
}
