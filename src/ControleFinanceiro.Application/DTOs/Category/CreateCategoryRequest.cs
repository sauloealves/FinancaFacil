using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleFinanceiro.Application.DTOs.Category {
    public class CreateCategoryRequest {
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; } 
    }
}
