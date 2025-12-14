using System.Security.Claims;
using API.DebugDojo.Application.Interfaces;

namespace API.DebugDojo.Api.Services;

public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _ctx;
    public CurrentUserAccessor(IHttpContextAccessor ctx) => _ctx = ctx;

    public bool IsAuthenticated => _ctx.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var sub = _ctx.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? _ctx.HttpContext?.User?.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }

    public string? Email => _ctx.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
}
