using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.DTOs.Account {
    public class BalanceResponse {
        public decimal Balance { get; set; }
        public DateTime ReferenceDate { get; set; }
    }
}
