namespace ControleFinanceiro.Domain.Entities;

public class BudgetMonth
{
    public Guid Id { get; private set; }
    public Guid BudgetId { get; private set; }
    public int Month { get; private set; } // 1-12

    // Navigation properties
    public Budget Budget { get; private set; } = null!;
    public ICollection<BudgetItem> Items { get; private set; } = new List<BudgetItem>();

    private BudgetMonth() { }

    public BudgetMonth(Guid budgetId, int month)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Month must be between 1 and 12");

        Id = Guid.NewGuid();
        BudgetId = budgetId;
        Month = month;
    }

    // Novo método para inicializar categorias
    public void InitializeCategories(List<Guid> categoryIds)
    {
        foreach (var categoryId in categoryIds)
        {
            Items.Add(new BudgetItem(Id, categoryId, 0, null));
        }
    }
}