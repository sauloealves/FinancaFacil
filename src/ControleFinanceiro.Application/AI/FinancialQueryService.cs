using ControleFinanceiro.Application.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.AI {
    public class FinancialQueryService {
        private readonly ITransactionRepository _transactionRepository;

        public FinancialQueryService(ITransactionRepository transactionRepository) {
            _transactionRepository = transactionRepository;
        }
        public async Task<object> Execute(Guid userId, AiIntentResult intent) {
            switch(intent.Intent) {
                case "monthly_expense":
                    return await GetMonthlyExpense(userId, intent);

                case "category_expense":
                    return await GetCategoryExpense(userId, intent);

                case "account_balance":
                    return await GetAccountBalance(userId, intent);

                case "monthly_income":
                    return await GetMonthlyIncome(userId, intent);

                default:
                    throw new Exception("Intent not supported");
            }
        }

        private async Task<object> GetMonthlyExpense(Guid userId, AiIntentResult intent) {
            var month = intent.Month ?? DateTime.Today.Month;
            var year = intent.Year ?? DateTime.Today.Year;

            var value = await _transactionRepository.GetExpenseByMonth(userId, month, year);

            return new {
                type = "monthly_expense",
                month,
                year,
                total = value
            };
        }

        private async Task<object> GetCategoryExpense(Guid userId, AiIntentResult intent) {
            if(string.IsNullOrEmpty(intent.Category))
                throw new Exception("Category required");

            var value = await _transactionRepository.GetExpenseByCategory(userId, Guid.Parse(intent.Category));

            return new {
                type = "category_expense",
                category = intent.Category,
                total = value
            };
        }

        private async Task<object> GetAccountBalance(Guid userId, AiIntentResult intent) {
            if(intent.AccountId == null)
                throw new Exception("Account required");

            var value = await _transactionRepository.GetAccountBalanceAsync(userId, intent.AccountId.Value);

            return new {
                type = "account_balance",
                accountId = intent.AccountId,
                balance = value
            };
        }

        private async Task<object> GetMonthlyIncome(Guid userId, AiIntentResult intent) {
            var month = intent.Month ?? DateTime.Today.Month;
            var year = intent.Year ?? DateTime.Today.Year;

            var value = await _transactionRepository.GetIncomeByMonth(userId, month, year);

            return new {
                type = "monthly_income",
                month,
                year,
                total = value
            };
        }

    }
}
