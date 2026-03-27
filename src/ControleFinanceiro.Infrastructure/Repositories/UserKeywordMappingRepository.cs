using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
    
namespace ControleFinanceiro.Infrastructure.Repositories;

public sealed class UserKeywordMappingRepository : IUserKeywordMappingRepository
{
    private readonly AppDbContext _context;

    public UserKeywordMappingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserKeywordMapping?> FindAccountByKeywordAsync(Guid userId, string keyword)
    {
        var normalized = keyword.ToLowerInvariant().Trim();

        return await _context.UserKeywordMappings
            .AsNoTracking()
            .FirstOrDefaultAsync(m => 
                m.UserId == userId && 
                m.AccountId != Guid.Empty &&
                m.Keyword.ToLower() == normalized);
    }

    public async Task<UserKeywordMapping?> FindCategoryByKeywordAsync(Guid userId, string keyword)
    {
        var normalized = keyword.ToLowerInvariant().Trim();

        return await _context.UserKeywordMappings
            .AsNoTracking()
            .FirstOrDefaultAsync(m => 
                m.UserId == userId && 
                m.CategoryId != Guid.Empty &&
                m.Keyword.ToLower() == normalized);
    }

    public async Task AddAsync(UserKeywordMapping mapping)
    {
        await _context.UserKeywordMappings.AddAsync(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<UserKeywordMapping>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserKeywordMappings
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserKeywordMapping?> GetByIdAsync(Guid id)
    {
        return await _context.UserKeywordMappings
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var mapping = await _context.UserKeywordMappings.FindAsync(id);
        if (mapping != null)
        {
            _context.UserKeywordMappings.Remove(mapping);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid userId, string keyword)
    {
        var normalized = keyword.ToLowerInvariant().Trim();

        return await _context.UserKeywordMappings
            .AnyAsync(m => 
                m.UserId == userId && 
                m.Keyword.ToLower() == normalized);
    }
}
