using ControleFinanceiro.Application.UseCases.Accounts;
using ControleFinanceiro.Application.UseCases.Auth;
using ControleFinanceiro.Application.UseCases.Categories;
using ControleFinanceiro.Application.UseCases.Transactions;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application {
    public static class DependencyInjection {
        public static IServiceCollection AddApplication(this IServiceCollection services) {
            services.AddScoped<RegisterUserUseCase>();
            services.AddScoped<LoginUserUseCase>();
            services.AddScoped<ForgotPasswordUseCase>();
            services.AddScoped<ResetPasswordUseCase>();
            services.AddScoped<CreateTransactionUseCase>();
            services.AddScoped<CreateAccountUseCase>();
            services.AddScoped<GetAccountsUseCase>();
            services.AddScoped<CreateCategoryUseCase>();
            services.AddScoped<DeleteAccountUseCase>();
            services.AddScoped<UpdateAccountUseCase>();
            return services;
        }
    }
}
