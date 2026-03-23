using ControleFinanceiro.Application.DTOs.Invoice;
using ControleFinanceiro.Application.Interfaces;

using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.UseCases.Invoice {
    public class AiParserService: IAiParserService {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AiParserService(HttpClient httpClient, IConfiguration config) {
            _httpClient = httpClient;
            _apiKey = config["OpenAI:ApiKey"];
        }

        public async Task<InvoiceResult> ParseAsync(string text) {
            var prompt = BuildPrompt(text);

            var request = new {
                model = "gpt-4.1-mini",
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                temperature = 0
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            var json = await response.Content.ReadAsStringAsync();

            var content = JsonDocument.Parse(json)
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            // extrai só o JSON dentro do ```json ... ```
            var match = Regex.Match(content, "```json\\s*(\\{.*?\\})\\s*```", RegexOptions.Singleline);
            content = match.Groups[1].Value;
            return JsonSerializer.Deserialize<InvoiceResult>(content);
        }

        private string BuildPrompt(string text) {
            return $@"
                Você é um parser de faturas de cartão de crédito.

                Extraia TODAS as transações da fatura abaixo.

                Retorne SOMENTE um JSON no formato:

                {{
                  ""transactions"": [
                    {{
                      ""date"": ""YYYY-MM-DD"",
                      ""description"": ""string"",
                      ""amount"": number
                    }}
                  ]
                }}

                Regras:
                - Datas no formato ISO
                - Valores negativos para despesas
                - Ignore totais e resumos
                - Não invente dados

                FATURA:
                '{ text}'
                ";
        }
    }
}
