using ControleFinanceiro.Application.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ControleFinanceiro.Application.AI
{
    public class AiIntentService
    {
        private readonly IAiClient _client;

        public AiIntentService(IAiClient client)
        {
            _client = client;
        }

        public async Task<AiIntentResult> ParseIntent(string question)
        {
            var prompt = $@"
            Você é um assistente financeiro.

            Converta a pergunta do usuário em JSON.

            Tipos de intent possíveis:

            - category_expense
            - account_balance
            - balance_projection
            - monthly_expense
            - monthly_income

            Regras:

            - Se o usuário perguntar gasto em categoria → category_expense
            - Se perguntar saldo → account_balance
            - Se perguntar saldo futuro → balance_projection
            - Se perguntar gasto do mês → monthly_expense
            - Se perguntar renda do mês → monthly_income

            Sempre retorne JSON válido no formato e sempre retorne o número do mês, não a nome:

            {{
              ""intent"": """",
              ""category"": """",
              ""accountId"": null,
              ""date"": null,
              ""month"": null,
              ""year"": null
            }}

            Pergunta: {question}
            ";

            var response = await _client.AskAsync(prompt, true);

            response = response.Replace("```json", "")
                       .Replace("```", "")
                       .Trim();

            return JsonSerializer.Deserialize<AiIntentResult>(
                response,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true
                }
            )!;
        }

        public async Task<string> AIResult(string question, object aiIntent)
        {
            var json = JsonSerializer.Serialize(aiIntent);

            var prompt = $"""
                Você é um assistente financeiro.

                O usuário fez a pergunta:

                "{question}"

                O sistema retornou os dados:

                {json}

                Explique o resultado de forma clara e amigável.
                Use valores em reais (R$).

                Retorne apenas a resposta.
                """;

            var response = await _client.AskAsync(prompt);
            return response;
        }

        public async Task<string?> ExtractCategoryAccountFromText(string text, List<string> availableCategories, List<string> availableAccount)
        {
            if (!availableCategories.Any())
                return null;

            var categoryNames = string.Join(", ", availableCategories);
            var accountNames = string.Join(", ", availableAccount);

            var prompt = $@"Você é um assistente especializado em extrair informações financeiras.

            TEXTO DO USUÁRIO: ""{text}""

            CATEGORIAS DISPONÍVEIS: {categoryNames}

            CONTAS DISPONÍVEIS: {accountNames}

            INSTRUÇÕES:
            - Extraia SOMENTE a categoria e a conta mencionada no texto
            - Se o texto mencionar uma conta, ignore a categoria e retorne NONE
            - IGNORE palavras genéricas como: receita, despesa, entrada, saída, gasto, pagamento, recebimento, conta, valor, dia, hoje, insira
            - Retorne EXATAMENTE o nome da categoria da lista acima que melhor se encaixa
            - Se não encontrar nenhuma categoria válida, retorne: NONE

            RESPONDA COM APENAS O NOME DA CATEGORIA E CONTA, SEPARADAS POR VIRGULA, SE UM DOS ITENS NAO VIER CONTA, OU SEM CATEGORIA COLOCAR NONE, E CATEGORIA VEM PRIMEIRO SEM EXPLICAÇÕES, PONTUAÇÃO EXTRA OU FORMATAÇÃO JSON.";

            try
            {
                var response = await _client.AskAsync(prompt);
                var extractedCategory = response.Split(',')[0]?.Trim().Trim('"', '\'', '.', ',', '`');
                var extractedAccount = response.Split(',')[1]?.Trim().Trim('"', '\'', '.', ',', '`');

                var matchedCategory = extractedAccount == "NONE" ? string.Empty : availableCategories.FirstOrDefault(c => 
                    c.Equals(extractedCategory, StringComparison.OrdinalIgnoreCase));

                var matchedAccount = extractedAccount == "NONE" ? string.Empty : availableAccount.FirstOrDefault(a => 
                    a.Equals(extractedAccount, StringComparison.OrdinalIgnoreCase));

                return matchedCategory + ", " + matchedAccount;
            }
            catch
            {
                return null;
            }
        }
    }

}
