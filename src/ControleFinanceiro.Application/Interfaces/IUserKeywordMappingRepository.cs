using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Interfaces;

public interface IUserKeywordMappingRepository
{
    Task<UserKeywordMapping?> FindAccountByKeywordAsync(Guid userId, string keyword);
    Task<UserKeywordMapping?> FindCategoryByKeywordAsync(Guid userId, string keyword);
    Task AddAsync(UserKeywordMapping mapping);
    Task<IEnumerable<UserKeywordMapping>> GetByUserIdAsync(Guid userId);
}