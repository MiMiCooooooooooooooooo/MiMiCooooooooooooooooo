using Microsoft.AspNetCore.Mvc;

namespace MoneyManager.Api.Controllers;

[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    [HttpPost("gmail-webhook")]
    public IActionResult GmailWebhook([FromBody] object payload)
    {
        return Ok(new
        {
            Message = "Webhook qabul qilindi. Bu joyda Gmail va statistika boti integratsiyasi amalga oshiriladi.",
            Payload = payload
        });
    }
}
