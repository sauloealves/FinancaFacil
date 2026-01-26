using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.ValueObjetcs {
    public class PasswordHash {
        public string Value { get; }

        private PasswordHash() { }

        public PasswordHash(string hash) {
            if(string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Hash da senha é obrigatório.");

            Value = hash;
        }

        public override string ToString() => Value;
    }
}
