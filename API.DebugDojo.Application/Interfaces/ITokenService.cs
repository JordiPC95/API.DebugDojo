using API.DebugDojo.Application.Entities;

namespace API.DebugDojo.Application.Interfaces;

public interface ITokenService
{
    (string token, DateTime expiresAtUtc) CreateAccessToken(User user);
}
