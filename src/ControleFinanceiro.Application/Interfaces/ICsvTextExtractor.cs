using ControleFinanceiro.Application.DTOs.Transaction;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Interfaces {
    public interface ICsvTextExtractor {
        Task<List<TransactionDto>> ExtractTextAsync(Stream csvStream);
    }
}
