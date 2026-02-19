using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.DTOs {
    public class AccountResponseDTO {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal InitialBalance { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
