using ControleFinanceiro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Infrastructure.Persistence {
    public class AppDbContext : DbContext {
        public DbSet<User> Users => Set<User>();
        public DbSet<PasswordResetToken> PasswordResetToken => Set<PasswordResetToken>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Category> Categories => Set<Category>();    
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<UserKeywordMapping> UserKeywordMappings => Set<UserKeywordMapping>();
        public DbSet<FailedTransaction> FailedTransactions => Set<FailedTransaction>();
        
        // Novos DbSets para Budget
        public DbSet<Budget> Budgets => Set<Budget>();
        public DbSet<BudgetMonth> BudgetMonths => Set<BudgetMonth>();
        public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) {
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) {
            configurationBuilder
                .Properties<DateTime>()
                .HaveColumnType("timestamp without time zone");

            configurationBuilder
                .Properties<DateTime?>()
                .HaveColumnType("timestamp without time zone");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
