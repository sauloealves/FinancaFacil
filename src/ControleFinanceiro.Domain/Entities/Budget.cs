using ControleFinanceiro.Domain.Enums;

namespace ControleFinanceiro.Domain.Entities;

public class Budget
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public BudgetStatus Status { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; } // Adicionar

    // Navigation properties
    public ICollection<BudgetMonth> Months { get; private set; } = new List<BudgetMonth>();

    private Budget() { }

    public Budget(Guid userId, string name, int year)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Year = year;
        Status = BudgetStatus.Active;
        CreatedAt = DateTime.Now;
        IsDeleted = false; // Adicionar

        // Criar automaticamente 12 meses
        InitializeMonths();
    }

    private void InitializeMonths()
    {
        for (int month = 1; month <= 12; month++)
        {
            Months.Add(new BudgetMonth(Id, month));
        }
    }

    // Novo método para adicionar categorias aos meses
    public void InitializeCategoriesForAllMonths(List<Guid> categoryIds)
    {
        foreach (var month in Months)
        {
            month.InitializeCategories(categoryIds);
        }
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateStatus(BudgetStatus status)
    {
        Status = status;
    }

    public void Delete() // Adicionar método
    {
        IsDeleted = true;
    }

    public void Restore() // Adicionar método (opcional)
    {
        IsDeleted = false;
    }
}