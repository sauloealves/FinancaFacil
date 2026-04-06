using ControleFinanceiro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Year)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.IsDeleted) // Adicionar
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => new { b.UserId, b.Year });
        builder.HasIndex(b => b.IsDeleted); // Adicionar índice

        // Relationships
        builder.HasMany(b => b.Months)
            .WithOne(m => m.Budget)
            .HasForeignKey(m => m.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query Filter global para soft delete
        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}