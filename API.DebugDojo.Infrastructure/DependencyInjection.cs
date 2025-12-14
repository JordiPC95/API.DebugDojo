using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.DebugDojo.Application.Interfaces;
using API.DebugDojo.Infrastructure.Data;
using API.DebugDojo.Infrastructure.Services;

namespace API.DebugDojo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("Default")));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWorkItemService, WorkItemService>();
        services.AddSingleton<ITokenService, TokenService>();

        return services;
    }
}
