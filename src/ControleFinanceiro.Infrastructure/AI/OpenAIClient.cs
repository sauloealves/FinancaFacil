using Microsoft.Extensions.Configuration;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ControleFinanceiro.Infrastructure.AI {
    public class OpenAIClient {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public OpenAIClient(
            HttpClient httpClient,
            IConfiguration configuration) {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"]!;
            _model = configuration["OpenAI:Model"]!;
        }

        public async Task<string> AskAsync(string prompt) {
            var request = new {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "You are a financial assistant." },
                    new { role = "user", content = prompt }
                }
            };

            var json = JsonSerializer.Serialize(request);

            var message = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions");

            message.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            message.Content =
                new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(message);

            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);

            return doc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!;
        }
    }
}
