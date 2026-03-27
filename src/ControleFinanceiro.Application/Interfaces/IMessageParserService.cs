using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Interfaces {
    public interface IMessageParserService {
        ParsedMessage Parse(string text, Guid userId, IEnumerable<Account> accounts, IEnumerable<Category> categories);
        Task<ParsedMessage> ParseAsync(Guid userId, string text, IEnumerable<Account> accounts, IEnumerable<Category> categories);
    }
}
