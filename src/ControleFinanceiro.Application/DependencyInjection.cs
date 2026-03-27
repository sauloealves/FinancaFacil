using ControleFinanceiro.Application.AI;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases.Accounts;
using ControleFinanceiro.Application.UseCases.Auth;
using ControleFinanceiro.Application.UseCases.Categories;
using ControleFinanceiro.Application.UseCases.FailedTransactions;
using ControleFinanceiro.Application.UseCases.Transactions;
using ControleFinanceiro.Application.UseCases.Whatsapp;

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
            services.AddScoped<UpdateCategoryUseCase>();
            services.AddScoped<DeleteCategoryUseCase>();
            services.AddScoped<GetCategoriesUseCase>();
            services.AddScoped<GetTransactionsUseCase>();
            services.AddScoped<GetBalanceUseCase>();
            services.AddScoped<UpdateTransactionUseCase>();
            services.AddScoped<AiIntentService>();
            services.AddScoped<FinancialQueryService>();
            services.AddScoped<DeleteTransactionUseCase>();
            services.AddScoped<CreateBatchTransactionUseCase>();
            services.AddScoped<IInvoiceImportService, InvoiceImportService>();
            services.AddScoped<ChangePasswordUseCase>();
            services.AddScoped<ProcessWhatsappMessageUseCase>();
            services.AddScoped<ResolveFailedTransactionUseCase>();
            services.AddScoped<ListFailedTransactionsUseCase>();
            return services;
        }
    }
}
