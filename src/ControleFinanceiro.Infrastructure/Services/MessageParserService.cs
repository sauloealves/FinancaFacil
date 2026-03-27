using ControleFinanceiro.Application.AI;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;
using System.Globalization;
using System.Text.RegularExpressions;

public class MessageParserService : IMessageParserService
{
    private readonly IUserKeywordMappingRepository _keywordRepo;
    private readonly IAccountRepository _accountRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly AiIntentService _aiIntentService; // Adicionar

    public MessageParserService(
        IUserKeywordMappingRepository keywordRepo,
        IAccountRepository accountRepo,
        ICategoryRepository categoryRepo,
        AiIntentService aiIntentService) // Adicionar
    {
        _keywordRepo = keywordRepo;
        _accountRepo = accountRepo;
        _categoryRepo = categoryRepo;
        _aiIntentService = aiIntentService; // Adicionar
    }
    public ParsedMessage Parse(string text, Guid userId, IEnumerable<Account> accounts, IEnumerable<Category> categories)
    {
        var originalText = text;
        text = text.ToLower();

        var parsed = new ParsedMessage
        {
            RawText = originalText,
            Amount = ExtractAmount(text),
            Date = ExtractDate(text),
            Type = DetectType(text)
        };

        var accountCategory = ExtractCategoryAccountAsync(userId, text, accounts, categories).GetAwaiter().GetResult();

        if (accountCategory != null)
        {
            var parts = accountCategory.Split(',');
            parsed.CategoryText = parts[0].Trim();
            parsed.AccountText = parts[1].Trim();
        }

        return parsed;
    }

    public async Task<ParsedMessage> ParseAsync(Guid userId, string text, IEnumerable<Account> accounts, IEnumerable<Category> categories)
    {
        var originalText = text;
        text = text.ToLower();

        var parsed = new ParsedMessage
        {
            RawText = originalText,
            Amount = ExtractAmount(text),
            Date = ExtractDate(text),
            Type = DetectType(text)
        };

        return parsed;
    }

    private decimal? ExtractAmount(string text)
    {
        // Match patterns: "365,00" | "365.00" | "365" | "R$ 365,00" | "365 reais"
        var patterns = new[]
        {
            @"r\$\s*(\d{1,3}(?:\.\d{3})*,\d{2})",           // R$ 1.234,56
            @"(\d{1,3}(?:\.\d{3})*,\d{2})",                  // 1.234,56
            @"r\$\s*(\d+(?:\.\d{3})*(?:,\d{2})?)",           // R$ 365 or R$ 365,00
            @"(\d+)\s*reais?",                                // 365 reais
            @"(\d+(?:,\d{2})?)"                               // 365 or 365,50
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(text, pattern);
            if (match.Success)
            {
                var valueStr = match.Groups[1].Value
                    .Replace(".", "")  // Remove thousand separator
                    .Replace(",", "."); // Convert to decimal separator

                if (decimal.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                    return value;
            }
        }

        return null;
    }

    private DateTime? ExtractDate(string text)
    {
        var today = DateTime.Today;

        // 1. Datas relativas específicas
        if (text.Contains("hoje"))
            return today;
        if (text.Contains("amanhã") || text.Contains("amanha"))
            return today.AddDays(1);
        if (text.Contains("depois de amanhã") || text.Contains("depois de amanha"))
            return today.AddDays(2);
        if (text.Contains("ontem"))
            return today.AddDays(-1);
        if (text.Contains("anteontem"))
            return today.AddDays(-2);

        // 2. PRIORIDADE: Expressões relativas com "dia X do mês" (DEVE VIR ANTES DO "dia X" simples)
        // Padrões: "dia 15 do próximo mês", "dia 12 desse mês", "dia 10 do mês passado"
        var relativeDayMatch = Regex.Match(text, @"dia\s+(\d{1,2})\s+do\s+(próximo|proximo|pr[oó]ximo|esse|desse|passado|anterior)\s+m[eê]s", RegexOptions.IgnoreCase);
        if (relativeDayMatch.Success)
        {
            var day = int.Parse(relativeDayMatch.Groups[1].Value);
            var monthModifier = relativeDayMatch.Groups[2].Value.ToLowerInvariant();

            var targetMonth = today.Month;
            var targetYear = today.Year;

            if (monthModifier.Contains("próx") || monthModifier.Contains("prox"))
            {
                targetMonth++;
                if (targetMonth > 12)
                {
                    targetMonth = 1;
                    targetYear++;
                }
            }
            else if (monthModifier == "passado" || monthModifier == "anterior")
            {
                targetMonth--;
                if (targetMonth < 1)
                {
                    targetMonth = 12;
                    targetYear--;
                }
            }
            // "esse" ou "desse" mantém o mês atual

            try
            {
                return new DateTime(targetYear, targetMonth, day);
            }
            catch
            {
                return today; // Fallback se a data for inválida
            }
        }

        // 3. Data completa: "15/05/2025" ou "15-05-2025"
        var fullDateMatch = Regex.Match(text, @"(\d{2})[/-](\d{2})[/-](\d{4})");
        if (fullDateMatch.Success)
        {
            var day = int.Parse(fullDateMatch.Groups[1].Value);
            var month = int.Parse(fullDateMatch.Groups[2].Value);
            var year = int.Parse(fullDateMatch.Groups[3].Value);

            try
            {
                return new DateTime(year, month, day);
            }
            catch
            {
                return today; // Fallback
            }
        }

        // 4. Data parcial: "15/05" (sem ano) -> usa ano atual
        var partialDateMatch = Regex.Match(text, @"(\d{2})[/-](\d{2})(?![/-]\d)");
        if (partialDateMatch.Success)
        {
            var day = int.Parse(partialDateMatch.Groups[1].Value);
            var month = int.Parse(partialDateMatch.Groups[2].Value);
            var year = today.Year;

            try
            {
                var parsedDate = new DateTime(year, month, day);
                
                // Se a data já passou este ano, assume que é para o próximo ano
                if (parsedDate < today)
                    parsedDate = parsedDate.AddYears(1);

                return parsedDate;
            }
            catch
            {
                return today; // Fallback
            }
        }

        // 5. Apenas dia: "dia 15" -> usa mês e ano atuais
        var dayOnlyMatch = Regex.Match(text, @"dia\s+(\d{1,2})(?!\s+do\s+)");
        if (dayOnlyMatch.Success)
        {
            var day = int.Parse(dayOnlyMatch.Groups[1].Value);
            var month = today.Month;
            var year = today.Year;

            try
            {
                var parsedDate = new DateTime(year, month, day);

                // Se o dia já passou neste mês, assume o próximo mês
                if (parsedDate < today)
                {
                    month++;
                    if (month > 12)
                    {
                        month = 1;
                        year++;
                    }
                    parsedDate = new DateTime(year, month, day);
                }

                return parsedDate;
            }
            catch
            {
                return today; // Fallback
            }
        }

        // 6. Expressões do tipo "próximo dia X", "próxima segunda", etc.
        var nextDayMatch = Regex.Match(text, @"pr[oó]xim[oa]\s+(segunda|terça|ter[cç]a|quarta|quinta|sexta|s[aá]bado|domingo)", RegexOptions.IgnoreCase);
        if (nextDayMatch.Success)
        {
            var dayOfWeek = nextDayMatch.Groups[1].Value.ToLowerInvariant();
            var targetDayOfWeek = MapDayOfWeek(dayOfWeek);

            return GetNextWeekday(today, targetDayOfWeek);
        }

        // 7. Expressões do tipo "semana que vem", "mês que vem", "ano que vem"
        if (text.Contains("semana que vem") || text.Contains("pr[oó]xima semana"))
            return today.AddDays(7);
        
        if (text.Contains("mês que vem") || text.Contains("m[eê]s que vem") || text.Contains("pr[oó]ximo m[eê]s"))
            return today.AddMonths(1);
        
        if (text.Contains("ano que vem") || text.Contains("pr[oó]ximo ano"))
            return today.AddYears(1);

        // 8. Default: hoje
        return today;
    }

    // Método auxiliar para mapear dia da semana
    private DayOfWeek MapDayOfWeek(string dayName)
    {
        return dayName switch
        {
            "segunda" => DayOfWeek.Monday,
            "terça" or "terca" => DayOfWeek.Tuesday,
            "quarta" => DayOfWeek.Wednesday,
            "quinta" => DayOfWeek.Thursday,
            "sexta" => DayOfWeek.Friday,
            "sábado" or "sabado" => DayOfWeek.Saturday,
            "domingo" => DayOfWeek.Sunday,
            _ => DayOfWeek.Monday
        };
    }

    // Método auxiliar para obter o próximo dia da semana
    private DateTime GetNextWeekday(DateTime startDate, DayOfWeek targetDay)
    {
        var daysToAdd = ((int)targetDay - (int)startDate.DayOfWeek + 7) % 7;
        if (daysToAdd == 0)
            daysToAdd = 7; // Se for o mesmo dia, pega a próxima semana

        return startDate.AddDays(daysToAdd);
    }

    private TransactionType? DetectType(string text)
    {
        if (text.Contains("transferir") || text.Contains("transferencia"))
            return TransactionType.Transfer;
        if (text.Contains("recebi") || text.Contains("ganho") || text.Contains("receita") || text.Contains("dividendo"))
            return TransactionType.Income;
        if (text.Contains("paguei") || text.Contains("compra") || text.Contains("gastei") || 
            text.Contains("despesa") || text.Contains("pagar") || text.Contains("gasto"))
            return TransactionType.Expense;

        return null;
    }

    private string? ExtractAccountFallback(string text)
    {
        // Extract known bank keywords (fallback for sync Parse)
        var bankKeywords = new[] { "nubank", "inter", "itau", "bradesco", "santander", "caixa", "bb", "banco do brasil", "c6", "picpay", "mercado pago" };
        
        foreach (var keyword in bankKeywords)
        {
            if (text.Contains(keyword))
                return keyword;
        }

        // For transfers: "da conta X para a conta Y"
        var transferMatch = Regex.Match(text, @"(?:da\s+conta\s+|de\s+)(\w+)");
        if (transferMatch.Success)
            return transferMatch.Groups[1].Value;

        return text; // Fallback: full text for fuzzy matching
    }

    private async Task<string?> ExtractAccountAsync(Guid userId, string text)
    {
        // Load user's account keywords WITH account names
        var keywords = await _keywordRepo.GetByUserIdAsync(userId);
        var accounts = await _accountRepo.GetByUserIdAsync(userId);

        var accountKeywordMap = keywords
            .Where(k => k.AccountId != Guid.Empty)
            .Join(accounts,
                keyword => keyword.AccountId,
                account => account.Id,
                (keyword, account) => new { Keyword = keyword.Keyword.ToLowerInvariant(), AccountName = account.Name })
            .ToList();

        // Check if any user keyword exists in the message
        foreach (var mapping in accountKeywordMap)
        {
            if (text.Contains(mapping.Keyword))
                return mapping.AccountName; // Return actual account name
        }

        // Fallback: check common bank names
        return ExtractAccountFallback(text);
    }

    private async Task<string?> ExtractCategoryAccountAsync(Guid userId, string text, IEnumerable<Account> accounts, IEnumerable<Category> categories)
    {        
        if (!categories.Any())
            return text; // Fallback se não houver categorias

        // Usa IA para extrair a categoria do texto
        var categoryNames = categories.Select(c => c.Name).ToList();
        var extractedCategory = await _aiIntentService.ExtractCategoryAccountFromText(text, categoryNames, accounts.Select(a => a.Name).ToList());

        if (!string.IsNullOrEmpty(extractedCategory))
            return extractedCategory;

        // Fallback: Se IA não encontrou, retorna texto completo para fuzzy matching
        return text;
    }    
}
