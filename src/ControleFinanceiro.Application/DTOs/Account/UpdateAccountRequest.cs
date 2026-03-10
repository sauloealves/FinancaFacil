namespace ControleFinanceiro.Application.DTOs.Account
{
    public class UpdateAccountRequest
    {        
        public string Name { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; }
    }
}