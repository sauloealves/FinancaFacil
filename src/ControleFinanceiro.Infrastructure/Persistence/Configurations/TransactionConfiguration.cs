using ControleFinanceiro.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations {
    public class TransactionConfiguration :IEntityTypeConfiguration<Transaction> {
        public void Configure(EntityTypeBuilder<Transaction> builder) {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.Type)
                .IsRequired();

            builder.Property(t => t.Date)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(t => t.TransferGroupId);

            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.AccountId);
            builder.HasIndex(t => t.TransferGroupId);

            builder.HasOne<Account>()
                .WithMany()
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
