using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infrastructure.Repositories;

public class FailedTransactionRepository : IFailedTransactionRepository
{
    private readonly AppDbContext _context;

    public FailedTransactionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(FailedTransaction failedTransaction)
    {
        await _context.FailedTransactions.AddAsync(failedTransaction);
        await _context.SaveChangesAsync();
    }

    public async Task<FailedTransaction?> GetByIdAsync(Guid id, Guid userId)
    {
        return await _context.FailedTransactions
            .FirstOrDefaultAsync(ft => ft.Id == id && ft.UserId == userId);
    }

    public async Task<List<FailedTransaction>> GetByUserIdAsync(Guid userId, bool includeResolved = false)
    {
        var query = _context.FailedTransactions
            .Where(ft => ft.UserId == userId);

        if (!includeResolved)
            query = query.Where(ft => !ft.IsResolved);

        return await query
            .OrderByDescending(ft => ft.SuggestedDate)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}