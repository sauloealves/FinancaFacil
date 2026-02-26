using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.DTOs {
    public class CreateAccountRequest {
        public string Name { get; set; } = String.Empty;
        public decimal InitialBalance { get; set; }
    }
}
