using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Infrastructure.Persistence;
using ControleFinanceiro.Infrastructure.Repositories;
using ControleFinanceiro.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure {
    public static class DependencyInjection {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration) {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null
                    )
                ));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAiClient, OpenAiClient>();
            services.AddScoped<IMatchingService, MatchingService>();  
            services.AddScoped<IFailedTransactionRepository, FailedTransactionRepository>();
            services.AddScoped<IMessageParserService, MessageParserService>();
            services.AddScoped<IUserKeywordMappingRepository, UserKeywordMappingRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            return services;
        }
    }
}
