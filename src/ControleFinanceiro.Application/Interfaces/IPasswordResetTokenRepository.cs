using ControleFinanceiro.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.Interfaces {
    public interface IPasswordResetTokenRepository {
        Task AddAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetValidAsync(string token);
        Task UpdateAsync(PasswordResetToken token);
    }
}
