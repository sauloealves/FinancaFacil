using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Common {
    public static class StringDistance {
        public static string Normalize(string text) {
            if(string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToLowerInvariant();

            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach(var c in normalized) {
                var unicodeCategory = Char.GetUnicodeCategory(c);
                if(unicodeCategory != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static int LevenshteinDistance(string source, string target) {
            if(string.IsNullOrEmpty(source))
                return target?.Length ?? 0;

            if(string.IsNullOrEmpty(target))
                return source.Length;

            int n = source.Length;
            int m = target.Length;

            var previous = new int[m + 1];
            var current = new int[m + 1];

            for(int j = 0; j <= m; j++)
                previous[j] = j;

            for(int i = 1; i <= n; i++) {
                current[0] = i;

                for(int j = 1; j <= m; j++) {
                    int cost = source[i - 1] == target[j - 1] ? 0 : 1;

                    current[j] = Math.Min(
                        Math.Min(
                            current[j - 1] + 1,
                            previous[j] + 1
                        ),
                        previous[j - 1] + cost
                    );
                }

                var temp = previous;
                previous = current;
                current = temp;
            }

            return previous[m];
        }
    }
}
