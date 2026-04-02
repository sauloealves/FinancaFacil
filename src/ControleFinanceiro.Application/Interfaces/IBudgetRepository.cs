using ControleFinanceiro.Application.DTOs.Budgets;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Interfaces;

public interface IBudgetRepository
{
    Task<Budget> AddAsync(Budget budget);
    Task<Budget?> GetByIdAsync(Guid id, Guid userId);
    Task<List<Budget>> GetByUserIdAsync(Guid userId);
    Task<BudgetMonth?> GetBudgetMonthAsync(Guid budgetMonthId, Guid userId);
    Task<List<BudgetMonth>> GetBudgetMonthsAsync(Guid budgetId, Guid userId);
    Task<BudgetItem?> GetBudgetItemAsync(Guid id, Guid userId);
    Task AddBudgetItemAsync(BudgetItem item);
    Task UpdateBudgetItemAsync(BudgetItem item);
    Task DeleteBudgetItemAsync(BudgetItem item);
    Task<List<BudgetItemDetailResponse>> GetBudgetItemsWithRealizationAsync(Guid budgetId, int month, int year, Guid userId);
    Task<BudgetSummaryResponse> GetBudgetSummaryAsync(Guid budgetId, int year, Guid userId);
    Task DeleteAsync(Budget budget);
    Task<List<Guid>> GetUserActiveCategoryIdsAsync(Guid userId); 
    Task SaveChangesAsync();
    Task AddCategoryToActiveBudgetsAsync(Guid userId, Guid categoryId);
}