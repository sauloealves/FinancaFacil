using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Interfaces {
    public interface ICategoryRepository {
        Task AddAsync(Category category);
        Task<Category> GetByIdAsync(Guid id);
    }
}
