using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Application.UseCases.Whatsapp;

public class ProcessWhatsappMessageUseCase
{
    private readonly IMessageParserService _aiParser;
    private readonly IAccountRepository _accountRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IMatchingService _matching;
    private readonly ITransactionRepository _transaction;
    private readonly IFailedTransactionRepository _failedRepo;

    public ProcessWhatsappMessageUseCase(
        IMessageParserService aiParser,
        IAccountRepository accountRepo,
        ICategoryRepository categoryRepo,
        IMatchingService matching,
        ITransactionRepository transaction,
        IFailedTransactionRepository failedRepo)
    {
        _aiParser = aiParser;
        _accountRepo = accountRepo;
        _categoryRepo = categoryRepo;
        _matching = matching;
        _transaction = transaction;
        _failedRepo = failedRepo;
    }

    public async Task<Result<string>> ExecuteAsync(Guid userId, string text)
    {
        var accounts = await _accountRepo.GetByUserIdAsync(userId);
        var categories = await _categoryRepo.GetAllByUserIdAsync(userId);

        var parsed = _aiParser.Parse(text, userId, accounts, categories);

        if (parsed.Type == null || parsed.Amount == null)
        {
            parsed = _aiParser.Parse(text, userId, accounts, categories);
        }

        var account = await _matching.MatchAccountAsync(userId, parsed.AccountText ?? text, accounts);
        var category = await _matching.MatchCategoryAsync(userId, parsed.CategoryText ?? text, categories);

        var (suggestedAccount, accountConfidence) = _matching.MatchAccountWithConfidence(parsed.AccountText ?? text, accounts);
        var (suggestedCategory, categoryConfidence) = _matching.MatchCategoryWithConfidence(parsed.CategoryText ?? text, categories);

        if (parsed.Amount == null || parsed.Amount <= 0)
        {
            await SaveFailedAsync(userId, parsed, "Valor não identificado ou inválido", null, null);
            return Result<string>.Fail("Não consegui identificar o valor da transação.");
        }

        if (parsed.Type == null)
        {
            await SaveFailedAsync(userId, parsed, "Tipo de transação não identificado", suggestedAccount?.Id, suggestedCategory?.Id);
            return Result<string>.Fail("Não consegui identificar se é despesa, receita ou transferência.");
        }

        if (account == null)
        {
            await SaveFailedAsync(userId, parsed, "Conta não encontrada", suggestedAccount?.Id, suggestedCategory?.Id);
            return Result<string>.Fail($"Não encontrei a conta mencionada. Sugestão: {suggestedAccount?.Name ?? "nenhuma"} (confiança: {accountConfidence:P0})");
        }

        if (parsed.Type != TransactionType.Transfer && category == null)
        {
            await SaveFailedAsync(userId, parsed, "Categoria não encontrada", account.Id, suggestedCategory?.Id);
            return Result<string>.Fail($"Não encontrei a categoria. Sugestão: {suggestedCategory?.Name ?? "nenhuma"} (confiança: {categoryConfidence:P0})");
        }

        if (parsed.Type == TransactionType.Transfer)
        {
            var destAccount = await _matching.MatchAccountAsync(userId, parsed.DestinationAccountText ?? "", accounts);
            if (destAccount == null || destAccount.Id == account.Id)
            {
                await SaveFailedAsync(userId, parsed, "Conta de destino não encontrada para transferência", account.Id, null);
                return Result<string>.Fail("Para transferências, preciso saber a conta de destino.");
            }

            // Create two transactions: expense from source, income to destination
            var expenseTransaction = new Transaction(
                userId,
                account.Id,
                parsed.Amount.Value,
                TransactionType.Expense,
                parsed.Date ?? DateTime.Today,
                $"Transferência para {destAccount.Name}",
                null,
                OccurrenceType.Single,
                null,
                null,
                null
            );

            var incomeTransaction = new Transaction(
                userId,
                destAccount.Id,
                parsed.Amount.Value,
                TransactionType.Income,
                parsed.Date ?? DateTime.Today,
                $"Transferência de {account.Name}",
                null,
                OccurrenceType.Single,
                null,
                null,
                null
            );

            await _transaction.AddAsync(expenseTransaction);
            await _transaction.AddAsync(incomeTransaction);

            return Result<string>.Ok($"✅ Transferência de R$ {parsed.Amount:N2} de {account.Name} para {destAccount.Name} registrada!");
        }

        // Create single transaction (expense or income)
        var transaction = new Transaction(
            userId,
            account.Id,
            parsed.Amount.Value,
            parsed.Type.Value,
            parsed.Date ?? DateTime.Today,
            parsed.RawText,
            category?.Id,
            OccurrenceType.Single,
            null,
            null,
            null
        );

        await _transaction.AddAsync(transaction);
    
        var typeLabel = parsed.Type == TransactionType.Expense ? "Despesa" : "Receita";
        return Result<string>.Ok($"✅ {typeLabel} de R$ {parsed.Amount:N2} em {account.Name} ({category?.Name ?? "sem categoria"}) registrada! No dia {parsed.Date:dd/MM/yyyy}");
    }

    private async Task SaveFailedAsync(Guid userId, ParsedMessage parsed, string reason, Guid? suggestedAccountId, Guid? suggestedCategoryId)
    {
        var transactionFailed = new FailedTransaction(
            userId,
            parsed.RawText,
            reason,
            parsed.Type,
            parsed.Amount,
            parsed.Date,
            suggestedAccountId,
            suggestedCategoryId
        );

        await _failedRepo.AddAsync(transactionFailed);
    }
}
