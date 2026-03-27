using ControleFinanceiro.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations {
    public class FailedTransactionsConfiguration : IEntityTypeConfiguration<FailedTransaction> {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FailedTransaction> builder) {
            builder.ToTable("FailedTransactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.OriginalText)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.SuggestedType);

            builder.Property(x => x.SuggestedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.SuggestedDate);

            builder.Property(x => x.SuggestedAccount);

            builder.Property(x => x.SuggestedCategory);

            builder.Property(x => x.Reason)
                .HasMaxLength(500);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.IsResolved)
                .IsRequired();

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => new { x.UserId, x.IsResolved });

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
