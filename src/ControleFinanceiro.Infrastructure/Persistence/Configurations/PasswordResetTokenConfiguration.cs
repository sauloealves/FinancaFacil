using ControleFinanceiro.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations {
    public class PasswordResetTokenConfiguration :IEntityTypeConfiguration<PasswordResetToken> {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Token).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.Used).IsRequired();
        }
    }
}
