namespace FinancaFacil.Application.DTOs
{
    public class UpdateAccountRequest
    {        
        public string Name { get; set; } = String.Empty;
        public decimal InitialBalance { get; set; }
    }
}