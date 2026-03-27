using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Interfaces;

public interface IMatchingService
{
    Task<Account?> MatchAccountAsync(Guid userId, string input, IEnumerable<Account> accounts);
    Task<Category?> MatchCategoryAsync(Guid userId, string input, IEnumerable<Category> categories);
    
    (Account? account, double confidence) MatchAccountWithConfidence(string input, IEnumerable<Account> accounts);
    (Category? category, double confidence) MatchCategoryWithConfidence(string input, IEnumerable<Category> categories);
}
