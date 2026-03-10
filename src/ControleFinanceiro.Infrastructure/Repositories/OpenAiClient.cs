using ControleFinanceiro.Application.Interfaces;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Repositories {
    public class OpenAiClient :IAiClient {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _model;

        public OpenAiClient(
            HttpClient httpClient,
            IConfiguration configuration) {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"]!;
            _model = configuration["OpenAI:Model"]!;
        }

        public async Task<string> AskAsync(string prompt, bool expectJson = false) {
            var request = new {
                model = _model,
                response_format = expectJson ? new {type = "json_object"}: null,
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


            if(!response.IsSuccessStatusCode) {
                throw new Exception($"OpenAI Error: {response.StatusCode} - {responseBody}");
            }

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
