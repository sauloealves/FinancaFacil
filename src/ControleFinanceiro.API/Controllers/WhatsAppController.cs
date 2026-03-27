using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Application.UseCases.Whatsapp;
using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.API.Controllers;

[ApiController]
[Route("api/whatsapp")]
public class WhatsAppController : ControllerBase
{
    private readonly ProcessWhatsappMessageUseCase _processWhatsappMessageUseCase;
    private readonly IUserRepository _userRepository;

    public WhatsAppController(ProcessWhatsappMessageUseCase processWhatsappMessageUseCase, IUserRepository userRepository)
    {
        _processWhatsappMessageUseCase = processWhatsappMessageUseCase;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var form = Request.Form;
        var message = form["Body"].ToString();
        var from = form["From"].ToString();

        var user = await _userRepository.GetByPhoneNumberAsync(from.Replace("+55","").Replace("whatsapp:",""));
        if (user == null)
        {
            return Ok(BuildTwiMLResponse("❌ Usuário não encontrado. Cadastre-se primeiro!"));
        }

        var result = await _processWhatsappMessageUseCase.ExecuteAsync(user.Id, message);

        var responseText = result.IsSuccess
            ? result.Data!
            : $"❌ {result.ErrorMessage}";

        Console.WriteLine($"[WhatsApp] {from}: {message} → {responseText}");

        return Ok(BuildTwiMLResponse(responseText));
    }

    private string BuildTwiMLResponse(string message)
    {
        return $@"<Response><Message>{System.Security.SecurityElement.Escape(message)}</Message></Response>";
    }
}
