using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Domain.Entities {
    public class Category {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public bool IsDeleted { get; private set; }
        public Guid? ParentCategoryId { get; set; }

        private Category() { }

        public Category(Guid userId, string name, Guid? parentCategoryId = null ) {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            IsDeleted = false;
            ParentCategoryId = parentCategoryId;
        }
    }
}
