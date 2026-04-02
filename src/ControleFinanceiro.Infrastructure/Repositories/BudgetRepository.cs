using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Enums;
using ControleFinanceiro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ControleFinanceiro.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly AppDbContext _context;

    public BudgetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Budget> AddAsync(Budget budget)
    {
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();
        return budget;
    }

    public async Task<Budget?> GetByIdAsync(Guid id, Guid userId)
    { 
        return await _context.Budgets
            .Include(b => b.Months)
                .ThenInclude(m => m.Items)
                    .ThenInclude(i => i.Category)
            .Include(b => b.Months)
                .ThenInclude(m => m.Items)
                    .ThenInclude(i => i.Account)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
    }

    public async Task<List<Budget>> GetByUserIdAsync(Guid userId)
    {        
        return await _context.Budgets
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<BudgetMonth?> GetBudgetMonthAsync(Guid budgetMonthId, Guid userId)
    {
        // Query Filter global já filtra IsDeleted = false no Budget
        return await _context.BudgetMonths
            .Include(bm => bm.Budget)
            .Include(bm => bm.Items)
                .ThenInclude(i => i.Category)
            .Include(bm => bm.Items)
                .ThenInclude(i => i.Account)
            .FirstOrDefaultAsync(bm => bm.Id == budgetMonthId && bm.Budget.UserId == userId);
    }

    public async Task<List<BudgetMonth>> GetBudgetMonthsAsync(Guid budgetId, Guid userId)
    {
        // Query Filter global já filtra IsDeleted = false no Budget
        return await _context.BudgetMonths
            .Include(bm => bm.Items)
                .ThenInclude(i => i.Category)
            .Include(bm => bm.Items)
                .ThenInclude(i => i.Account)
            .Where(bm => bm.BudgetId == budgetId && bm.Budget.UserId == userId)
            .OrderBy(bm => bm.Month)
            .ToListAsync();
    }

    public async Task<BudgetItem?> GetBudgetItemAsync(Guid id, Guid userId)
    {
        return await _context.BudgetItems
            .Include(bi => bi.BudgetMonth)
                .ThenInclude(bm => bm.Budget)
            .FirstOrDefaultAsync(bi => bi.Id == id && bi.BudgetMonth.Budget.UserId == userId);
    }

    public async Task AddBudgetItemAsync(BudgetItem item)
    {
        await _context.BudgetItems.AddAsync(item);
    }

    public async Task UpdateBudgetItemAsync(BudgetItem item)
    {
        _context.BudgetItems.Update(item);
    }

    public async Task DeleteBudgetItemAsync(BudgetItem item)
    {
        _context.BudgetItems.Remove(item);
    }

    public async Task<List<BudgetItemDetailResponse>> GetBudgetItemsWithRealizationAsync(
        Guid budgetId, 
        int month, 
        int year, 
        Guid userId)
    {
        // Query Filter global já filtra IsDeleted = false no Budget
        var items = await (
            from budgetItem in _context.BudgetItems
            join budgetMonth in _context.BudgetMonths on budgetItem.BudgetMonthId equals budgetMonth.Id
            join budget in _context.Budgets on budgetMonth.BudgetId equals budget.Id
            join category in _context.Categories on budgetItem.CategoryId equals category.Id
            where budget.Id == budgetId 
                && budget.UserId == userId 
                && budgetMonth.Month == month
                && !category.IsDeleted
            select new
            {
                budgetItem.Id,
                budgetItem.CategoryId,
                CategoryName = category.Name,
                budgetItem.AccountId,
                budgetItem.PlannedAmount
            })
            .ToListAsync();

        // Calcular valores realizados em uma única query
        var categoryIds = items.Select(i => i.CategoryId).Distinct().ToList();
        
        var realizedAmounts = await _context.Transactions
            .Where(t => t.UserId == userId
                && t.Date.Year == year
                && t.Date.Month == month
                && categoryIds.Contains(t.CategoryId!.Value)
                && t.Type == TransactionType.Expense
                && !t.IsDeleted)
            .GroupBy(t => new { t.CategoryId, t.AccountId })
            .Select(g => new
            {
                CategoryId = g.Key.CategoryId!.Value,
                AccountId = g.Key.AccountId,
                RealizedAmount = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        // Buscar nomes das contas
        var accountIds = items.Where(i => i.AccountId.HasValue).Select(i => i.AccountId!.Value).Distinct().ToList();
        var accounts = await _context.Accounts
            .Where(a => accountIds.Contains(a.Id) && !a.IsDeleted)
            .ToDictionaryAsync(a => a.Id, a => a.Name);

        // Combinar dados
        return items.Select(item =>
        {
            var realized = item.AccountId.HasValue
                ? realizedAmounts.FirstOrDefault(r => r.CategoryId == item.CategoryId && r.AccountId == item.AccountId)?.RealizedAmount ?? 0
                : realizedAmounts.Where(r => r.CategoryId == item.CategoryId).Sum(r => r.RealizedAmount);

            var difference = item.PlannedAmount - realized;
            var percentageUsed = item.PlannedAmount > 0 ? (realized / item.PlannedAmount) * 100 : 0;

            return new BudgetItemDetailResponse
            {
                Id = item.Id,
                CategoryId = item.CategoryId,
                CategoryName = item.CategoryName,
                AccountId = item.AccountId,
                AccountName = item.AccountId.HasValue && accounts.ContainsKey(item.AccountId.Value) 
                    ? accounts[item.AccountId.Value] 
                    : null,
                PlannedAmount = item.PlannedAmount,
                RealizedAmount = realized,
                Difference = difference,
                PercentageUsed = percentageUsed
            };
        }).ToList();
    }

    public async Task<BudgetSummaryResponse> GetBudgetSummaryAsync(Guid budgetId, int year, Guid userId)
    {
        // Query Filter global já filtra IsDeleted = false no Budget
        var budget = await _context.Budgets
            .Include(b => b.Months)
                .ThenInclude(m => m.Items)
                    .ThenInclude(i => i.Category)
            .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId);

        if (budget == null)
            throw new InvalidOperationException("Budget not found");

        // Total planejado
        var totalPlanned = budget.Months
            .SelectMany(m => m.Items)
            .Sum(i => i.PlannedAmount);

        // Buscar todos os IDs de categorias do orçamento
        var categoryIds = budget.Months
            .SelectMany(m => m.Items)
            .Select(i => i.CategoryId)
            .Distinct()
            .ToList();

        // Total realizado em uma query otimizada
        var totalRealized = await _context.Transactions
            .Where(t => t.UserId == userId
                && t.Date.Year == year
                && categoryIds.Contains(t.CategoryId!.Value)
                && t.Type == TransactionType.Expense
                && !t.IsDeleted)
            .SumAsync(t => (decimal?)t.Amount) ?? 0;

        // Breakdown por categoria
        var realizedByCategory = await _context.Transactions
            .Where(t => t.UserId == userId
                && t.Date.Year == year
                && categoryIds.Contains(t.CategoryId!.Value)
                && t.Type == TransactionType.Expense
                && !t.IsDeleted)
            .GroupBy(t => t.CategoryId!.Value)
            .Select(g => new
            {
                CategoryId = g.Key,
                RealizedAmount = g.Sum(t => t.Amount)
            })
            .ToDictionaryAsync(x => x.CategoryId, x => x.RealizedAmount);

        var categoryBreakdowns = budget.Months
            .SelectMany(m => m.Items)
            .GroupBy(i => i.CategoryId)
            .Select(g =>
            {
                var plannedAmount = g.Sum(i => i.PlannedAmount);
                var realizedAmount = realizedByCategory.ContainsKey(g.Key) 
                    ? realizedByCategory[g.Key] 
                    : 0;
                var difference = plannedAmount - realizedAmount;
                var percentageUsed = plannedAmount > 0 ? (realizedAmount / plannedAmount) * 100 : 0;

                return new CategoryBreakdown
                {
                    CategoryId = g.Key,
                    CategoryName = g.First().Category.Name,
                    PlannedAmount = plannedAmount,
                    RealizedAmount = realizedAmount,
                    Difference = difference,
                    PercentageUsed = percentageUsed
                };
            })
            .OrderByDescending(cb => cb.PlannedAmount)
            .ToList();

        var balance = totalPlanned - totalRealized;
        var percentageUsed = totalPlanned > 0 ? (totalRealized / totalPlanned) * 100 : 0;

        return new BudgetSummaryResponse
        {
            BudgetId = budget.Id,
            BudgetName = budget.Name,
            Year = budget.Year,
            TotalPlanned = totalPlanned,
            TotalRealized = totalRealized,
            Balance = balance,
            PercentageUsed = percentageUsed,
            CategoryBreakdowns = categoryBreakdowns
        };
    }

    public async Task DeleteAsync(Budget budget) // Adicionar método
    {
        budget.Delete();
        _context.Budgets.Update(budget);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Guid>> GetUserActiveCategoryIdsAsync(Guid userId)
    {
        return await _context.Categories
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .Select(c => c.Id)
            .ToListAsync();
    }

    public async Task<List<Budget>> GetActiveBudgetsByUserIdAsync(Guid userId)
    {
        return await _context.Budgets
            .AsNoTracking() // Adicionar AsNoTracking se for apenas leitura
            .Include(b => b.Months)
                .ThenInclude(m => m.Items)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .ToListAsync();
    }

    public async Task AddCategoryToActiveBudgetsAsync(Guid userId, Guid categoryId)
    {
        // 1. Buscar IDs dos BudgetMonths dos budgets ativos
        var budgetMonthIds = await _context.Budgets
            .AsNoTracking()
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .SelectMany(b => b.Months)
            .Select(m => m.Id)
            .ToListAsync();

        if (!budgetMonthIds.Any())
            return;

        // 2. Buscar quais meses já têm essa categoria
        var existingMonthIds = await _context.BudgetItems
            .Where(bi => budgetMonthIds.Contains(bi.BudgetMonthId) && bi.CategoryId == categoryId)
            .Select(bi => bi.BudgetMonthId)
            .ToListAsync();

        // 3. Criar BudgetItems apenas para os meses que não têm a categoria
        var monthIdsToAdd = budgetMonthIds.Where(id => !existingMonthIds.Contains(id)).ToList();

        if (!monthIdsToAdd.Any())
            return;

        var newBudgetItems = monthIdsToAdd
            .Select(monthId => new BudgetItem(monthId, categoryId, 0, null))
            .ToList();

        // 4. Adicionar em batch
        await _context.BudgetItems.AddRangeAsync(newBudgetItems);
        await _context.SaveChangesAsync();
    }
}