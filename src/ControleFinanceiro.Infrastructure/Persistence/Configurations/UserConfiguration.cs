using ControleFinanceiro.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations {
    public class UserConfiguration :IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.OwnsOne(u => u.Email, email => {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(200);

                email.HasIndex(e => e.Value).IsUnique();
            });

            builder.OwnsOne(u => u.PasswordHash, password => {
                password.Property(p => p.Value)
                    .HasColumnName("PasswordHash")
                    .IsRequired();
            });

            builder.Property(u => u.Active)
                .IsRequired();

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.Plan)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
