using AdminBot.Web.Handlers;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace AdminBot.Web.Controllers;

[ApiController]
[Route("/api/update")]
public class UpdateController : ControllerBase
{
    private readonly IUpdateHandler _updateHandler;

    public UpdateController(IUpdateHandler updateHandler)
    {
        _updateHandler = updateHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        var receivedAt = DateTime.UtcNow;
        
        await _updateHandler.HandleAsync(update, receivedAt);

        return this.Ok();
    }
}