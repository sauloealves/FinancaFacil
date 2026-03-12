using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Resend;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Services {
    public class EmailService :IEmailService {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IResend _resend;
        private readonly HttpClient _httpClient;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IResend resend, HttpClient httpClient) {
            _configuration = configuration;
            _logger = logger;
            _resend = resend;
            _httpClient = httpClient;
        }
        public async Task SendAsync(string to, string subject, string body) {
            /*try {
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
            }*/

            var apiKey = _configuration["Resend:ApiKey"];
            var from = _configuration["Resend:From"];

            _logger.LogInformation("Resend key length: {Length}", apiKey?.Length);

            var payload = new {
                from = from,
                to = new[] { to },
                subject = subject,
                html = body
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Resend response: {Response}", responseBody);

            response.EnsureSuccessStatusCode();

        }
    }
}
