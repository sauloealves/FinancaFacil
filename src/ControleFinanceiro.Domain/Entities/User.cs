using ControleFinanceiro.Domain.ValueObjetcs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Entities {
    public class User {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Email Email { get; private set; }
        public PasswordHash PasswordHash { get; private set; }
        public bool Active { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private User() { } // EF Core

        public User(string name, Email email, PasswordHash passwordHash) {
            Id = Guid.NewGuid();
            Name = name ?? throw new ArgumentException("Nome é obrigatório.");
            Email = email ?? throw new ArgumentException("E-mail é obrigatório.");
            PasswordHash = passwordHash ?? throw new ArgumentException("Senha é obrigatória.");
            Active = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deactivate() {
            Active = false;
        }

        public void ChangePassword(PasswordHash newPasswordHash) {
            PasswordHash = newPasswordHash
                ?? throw new ArgumentException("Nova senha inválida.");
        }
    }
}
