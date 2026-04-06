namespace ControleFinanceiro.Domain.Entities;

public class BudgetItem
{
    public Guid Id { get; private set; }
    public Guid BudgetMonthId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? AccountId { get; private set; }
    public decimal PlannedAmount { get; private set; }

    // Navigation properties
    public BudgetMonth BudgetMonth { get; private set; } = null!;
    public Category Category { get; private set; } = null!;
    public Account? Account { get; private set; }

    private BudgetItem() { }

    public BudgetItem(Guid budgetMonthId, Guid categoryId, decimal plannedAmount, Guid? accountId = null)
    {
        Id = Guid.NewGuid();
        BudgetMonthId = budgetMonthId;
        CategoryId = categoryId;
        AccountId = accountId;
        PlannedAmount = plannedAmount;
    }

    public void UpdatePlannedAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Planned amount cannot be negative");

        PlannedAmount = amount;
    }

    public void UpdateCategory(Guid categoryId)
    {
        CategoryId = categoryId;
    }

    public void UpdateAccount(Guid? accountId)
    {
        AccountId = accountId;
    }
}