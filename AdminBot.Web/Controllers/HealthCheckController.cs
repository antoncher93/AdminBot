using Microsoft.AspNetCore.Mvc;

namespace AdminBot.Web.Controllers;

[ApiController]
[Route("/api/healthcheck")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return this.Ok("AdminBot is OK");
    }
}