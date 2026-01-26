using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ControleFinanceiro.Application.Interfaces;

using Microsoft.AspNetCore.Identity;

namespace ControleFinanceiro.Infrastructure.Services {
    public class PasswordHasher: IPasswordHasher {
        
        private readonly PasswordHasher<object> _hasher = new();
        public string Hash(string password) {
            return _hasher.HashPassword(null!, password);
        }
    }
}
