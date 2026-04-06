using ControleFinanceiro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

public class BudgetMonthConfiguration : IEntityTypeConfiguration<BudgetMonth>
{
    public void Configure(EntityTypeBuilder<BudgetMonth> builder)
    {
        builder.HasKey(bm => bm.Id);

        builder.Property(bm => bm.BudgetId)
            .IsRequired();

        builder.Property(bm => bm.Month)
            .IsRequired();

        // Indexes
        builder.HasIndex(bm => bm.BudgetId);
        builder.HasIndex(bm => new { bm.BudgetId, bm.Month })
            .IsUnique();

        // Relationships
        builder.HasMany(bm => bm.Items)
            .WithOne(i => i.BudgetMonth)
            .HasForeignKey(i => i.BudgetMonthId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}