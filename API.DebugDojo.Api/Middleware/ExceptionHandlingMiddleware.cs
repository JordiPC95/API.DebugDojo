using System.Net;
using System.Text.Json;
using API.DebugDojo.Application.Exceptions;

namespace API.DebugDojo.Api.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", context.TraceIdentifier);

            var (status, title) = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
                ConflictException => (HttpStatusCode.Conflict, "Conflict"),
                UnauthorizedAppException => (HttpStatusCode.Unauthorized, "Unauthorized"),
                _ => (HttpStatusCode.InternalServerError, "Unexpected error")
            };

            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/problem+json";

            var problem = new
            {
                type = "about:blank",
                title,
                status = (int)status,
                detail = ex.Message,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
