using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.ValueObjetcs {
    public class Email {
        public string Value { get; }

        private Email() { }

        public Email(string value) {
            if(string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("E-mail é obrigatório.");

            if(!IsValid(value))
                throw new ArgumentException("E-mail inválido.");

            Value = value.ToLowerInvariant();
        }

        private bool IsValid(string email) {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        public override string ToString() => Value;
    }
}
