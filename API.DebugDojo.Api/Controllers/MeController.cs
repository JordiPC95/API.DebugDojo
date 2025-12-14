using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DebugDojo.Application.Interfaces;

namespace API.DebugDojo.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public class MeController : ControllerBase
{
    private readonly ICurrentUserAccessor _me;
    public MeController(ICurrentUserAccessor me) => _me = me;

    [HttpGet]
    public IActionResult Get() => Ok(new { userId = _me.UserId, email = _me.Email });
}
