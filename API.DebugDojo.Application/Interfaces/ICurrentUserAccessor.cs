namespace API.DebugDojo.Application.Interfaces;

public interface ICurrentUserAccessor
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
    string? Email { get; }
}
