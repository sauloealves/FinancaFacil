using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.DTOs {
    public class CreateCategoryRequest {
        public string Name { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public Guid? ParentCategoryId { get; set; } 
    }
}
