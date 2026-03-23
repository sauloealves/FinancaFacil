using ControleFinanceiro.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/invoices")]
    public class InvoiceController: ControllerBase {

        private readonly IInvoiceImportService _service;

        public InvoiceController(IInvoiceImportService service) {
            _service = service;
        }

        [HttpPost("import-invoice")]
        public async Task<IActionResult> Import([FromForm] IFormFile file) {
            if(file == null || file.Length == 0)
                return BadRequest("Arquivo inválido");

            using var stream = file.OpenReadStream();

            var extension = Path.GetExtension(file.FileName).ToLower();

            var result = await _service.ImportAsync(stream, extension);

            return Ok(result);
        }
    }
}
