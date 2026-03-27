using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Common {
    public class Similarity {
        public static double Calculate(string a, string b) {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return 0;

            int distance = StringDistance.LevenshteinDistance(a, b);
            int maxLength = Math.Max(a.Length, b.Length);

            return 1.0 - (double)distance / maxLength;
        }
    }
}
