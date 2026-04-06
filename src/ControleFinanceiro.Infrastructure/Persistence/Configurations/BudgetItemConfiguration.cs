using ControleFinanceiro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

public class BudgetItemConfiguration : IEntityTypeConfiguration<BudgetItem>
{
    public void Configure(EntityTypeBuilder<BudgetItem> builder)
    {
        builder.HasKey(bi => bi.Id);

        builder.Property(bi => bi.BudgetMonthId)
            .IsRequired();

        builder.Property(bi => bi.CategoryId)
            .IsRequired();

        builder.Property(bi => bi.PlannedAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        // Indexes para performance
        builder.HasIndex(bi => bi.BudgetMonthId);
        builder.HasIndex(bi => bi.CategoryId);
        builder.HasIndex(bi => bi.AccountId);
        builder.HasIndex(bi => new { bi.BudgetMonthId, bi.CategoryId });

        // Relationships
        builder.HasOne(bi => bi.Category)
            .WithMany()
            .HasForeignKey(bi => bi.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bi => bi.Account)
            .WithMany()
            .HasForeignKey(bi => bi.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}