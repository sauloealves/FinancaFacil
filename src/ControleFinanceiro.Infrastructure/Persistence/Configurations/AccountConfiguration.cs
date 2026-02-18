using ControleFinanceiro.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations {
    public class AccountConfiguration :IEntityTypeConfiguration<Account> {
        public void Configure(EntityTypeBuilder<Account> builder) {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.UserId)
                .IsRequired();

            builder.Property(a => a.InitialBalance)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(a => a.IsDeleted)
                .IsRequired();

            builder.HasIndex(a => new { a.UserId, a.Name });
        }
    }
}
