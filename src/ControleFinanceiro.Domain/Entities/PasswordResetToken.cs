using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Entities {
    public class PasswordResetToken {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool Used { get; private set; }

        private PasswordResetToken() { }

        public PasswordResetToken(Guid userId, string token, DateTime expiresAt) {
            Id = Guid.NewGuid();
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            Used = false;
        }

        public void MarkAsUsed() => Used = true;
    }
}
