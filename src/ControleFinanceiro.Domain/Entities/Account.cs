using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Entities {
    public class Account {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal InitialBalance { get; private set; }
        public bool IsDeleted { get; set; }

        public bool IsEnabled { get; set;}

        public Account() { }

        public Account(Guid userId, string name, decimal initialBalance) {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name ?? throw new ArgumentException("Name is required.");
            InitialBalance = initialBalance;
            IsDeleted = false;
            IsEnabled = true;
        }

        public void Update(string name, decimal initialBalance, bool isEnabled) {
            Name = name ?? throw new ArgumentException("Name is required.");
            InitialBalance = initialBalance;
            IsEnabled = isEnabled;
        }
    }
}
