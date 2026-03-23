using ControleFinanceiro.Application.DTOs.Invoice;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Interfaces {
    public interface IAiParserService {
        Task<InvoiceResult> ParseAsync(string query);
    }
}
