namespace ControleFinanceiro.Application.DTOs.Category {
    public class UpdateCategoryRequest {
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
    }
}
