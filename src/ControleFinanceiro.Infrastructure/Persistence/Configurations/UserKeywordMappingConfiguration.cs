using ControleFinanceiro.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Persistence.Configurations {
    public class UserKeywordMappingConfiguration: IEntityTypeConfiguration<UserKeywordMapping> {
        public UserKeywordMappingConfiguration() { }

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserKeywordMapping> builder) {
                builder.ToTable("UserKeywordMappings");
    
                builder.HasKey(ukm => ukm.Id);
    
                builder.Property(ukm => ukm.UserId)
                    .IsRequired();
    
                builder.Property(ukm => ukm.Keyword)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(ukm => ukm.AccountId)
                    .IsRequired();

                builder.Property(ukm => ukm.CategoryId)
                    .IsRequired();

            builder.HasIndex(ukm => new { ukm.UserId, ukm.Keyword }).IsUnique();
        }
    }
}
