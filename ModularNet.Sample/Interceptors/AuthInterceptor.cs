using ModularNet.Core.Attributes;
using ModularNet.Core.Context;
using ModularNet.Core.Enums;
using ModularNet.Core.Exceptions;
using ModularNet.Core.Interfaces;
using ExecutionContext = ModularNet.Core.Context.ExecutionContext;

namespace ModularNet.Sample.Interceptors;

[Injectable(ServiceScope.Scoped)]
public class AuthInterceptor : IInterceptor
{
    private readonly ILogger<AuthInterceptor> _logger;
    private const string API_KEY_HEADER = "X-API-Key";
    private const string VALID_API_KEY = "secret-api-key-12345";

    public AuthInterceptor(ILogger<AuthInterceptor> logger)
    {
        _logger = logger;
    }

    public async Task<object?> InterceptAsync(ExecutionContext context, CallHandler next)
    {
        var httpContext = context.HttpContext;

        // Check for API key in headers
        if (!httpContext.Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKey))
        {
            _logger.LogWarning("Request without API key to {Path}", httpContext.Request.Path);
            throw new UnauthorizedException("API key is required");
        }

        if (apiKey != VALID_API_KEY)
        {
            _logger.LogWarning("Invalid API key attempt: {ApiKey}", apiKey);
            throw new UnauthorizedException("Invalid API key");
        }

        _logger.LogInformation("Authenticated request to {Path}", httpContext.Request.Path);

        return await next.HandleAsync();
    }
}
