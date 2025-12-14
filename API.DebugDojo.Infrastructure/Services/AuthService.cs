using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using API.DebugDojo.Application.Contracts;
using API.DebugDojo.Application.Entities;
using API.DebugDojo.Application.Exceptions;
using API.DebugDojo.Application.Interfaces;
using API.DebugDojo.Infrastructure.Data;

namespace API.DebugDojo.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            throw new ConflictException("Ya existe un usuario con ese email.");

        var user = new User { Email = email };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var (token, exp) = _tokenService.CreateAccessToken(user);
        return new AuthResponse(token, exp);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email, ct);

        if (user is null)
            throw new UnauthorizedAppException("Credenciales inválidas.");

        var ok = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (ok == PasswordVerificationResult.Failed)
            throw new UnauthorizedAppException("Credenciales inválidas.");

        var (token, exp) = _tokenService.CreateAccessToken(user);
        return new AuthResponse(token, exp);
    }
}
