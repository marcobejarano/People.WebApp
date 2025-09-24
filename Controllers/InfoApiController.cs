using Microsoft.AspNetCore.Mvc;

namespace People.WebApp.Controllers;

[Route("api/info")]
[ApiController]
public class InfoApiController : ControllerBase
{
    [HttpGet]
    public string GetMachineName()
    {
        return $"The request was processed by: {Environment.MachineName}";
    }
}
