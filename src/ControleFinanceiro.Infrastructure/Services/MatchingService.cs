using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Common;
using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Infrastructure.Services {
    public class MatchingService : IMatchingService {
        private readonly IUserKeywordMappingRepository _keywordRepo;

        public MatchingService(IUserKeywordMappingRepository keywordRepo) {
            _keywordRepo = keywordRepo;
        }

        public async Task<Account?> MatchAccountAsync(Guid userId, string input, IEnumerable<Account> accounts) {
            input = StringDistance.Normalize(input);

            // Step 1: Check user's keyword mappings first (instant match)
            var keywordMatch = await _keywordRepo.FindAccountByKeywordAsync(userId, input);
            if (keywordMatch != null)
                return accounts.FirstOrDefault(a => a.Id == keywordMatch.AccountId);

            // Step 2: Fuzzy match against account names
            var best = accounts
                .Select(a =>
                {
                    var normalized = StringDistance.Normalize(a.Name);
                    var similarity = Similarity.Calculate(input, normalized);

                    return new { Account = a, Similarity = similarity };
                })
                .OrderByDescending(x => x.Similarity)
                .FirstOrDefault();

            if (best?.Similarity < 0.6)
                return null;

            return best.Account;
        }

        public async Task<Category?> MatchCategoryAsync(Guid userId, string input, IEnumerable<Category> categories) {
            input = StringDistance.Normalize(input);

            // Step 1: Check user's keyword mappings first
            var keywordMatch = await _keywordRepo.FindCategoryByKeywordAsync(userId, input);
            if (keywordMatch != null)
                return categories.FirstOrDefault(c => c.Id == keywordMatch.CategoryId);

            // Step 2: Fuzzy match against category names
            var best = categories
                .Select(c =>
                {
                    var normalized = StringDistance.Normalize(c.Name);
                    var similarity = Similarity.Calculate(input, normalized);

                    return new { Category = c, Similarity = similarity };
                })
                .OrderByDescending(x => x.Similarity)
                .FirstOrDefault();

            if (best?.Similarity < 0.6)
                return null;

            return best.Category;
        }

        // Helper: suggest best match even if below threshold (for failed transactions)
        public (Account? account, double confidence) MatchAccountWithConfidence(string input, IEnumerable<Account> accounts) {
            input = StringDistance.Normalize(input);

            var best = accounts
                .Select(a =>
                {
                    var normalized = StringDistance.Normalize(a.Name);
                    var similarity = Similarity.Calculate(input, normalized);
                    return new { Account = a, Similarity = similarity };
                })
                .OrderByDescending(x => x.Similarity)
                .FirstOrDefault();

            return best != null ? (best.Account, best.Similarity) : (null, 0);
        }

        public (Category? category, double confidence) MatchCategoryWithConfidence(string input, IEnumerable<Category> categories) {
            input = StringDistance.Normalize(input);

            var best = categories
                .Select(c =>
                {
                    var normalized = StringDistance.Normalize(c.Name);
                    var similarity = Similarity.Calculate(input, normalized);
                    return new { Category = c, Similarity = similarity };
                })
                .OrderByDescending(x => x.Similarity)
                .FirstOrDefault();

            return best != null ? (best.Category, best.Similarity) : (null, 0);
        }
    }
}
