using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MauiMessenger.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public ActionResult<object> Get()
    {
        return Ok(new
        {
            status = "ok",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}
