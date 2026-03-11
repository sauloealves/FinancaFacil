using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Constants {
    public static class DefaultData {
        public static readonly string[] Accounts =
        {
            "Dinheiro",
            "Cartão",
            "Conta-Corrente"
        };

        public static readonly string[] Categories =
        {
            "Alimentação",
            "Casa",
            "Diversão",
            "Diversos",
            "Educação",
            "Saúde",
            "Transporte",
            "Vestuário",
            "Outros"
        };
    }
}
