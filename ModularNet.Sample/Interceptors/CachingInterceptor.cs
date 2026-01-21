using ModularNet.Core.Attributes;
using ModularNet.Core.Context;
using ModularNet.Core.Enums;
using ModularNet.Core.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;
using ExecutionContext = ModularNet.Core.Context.ExecutionContext;

namespace ModularNet.Sample.Interceptors;

[Injectable(ServiceScope.Singleton)]
public class CachingInterceptor : IInterceptor
{
    private readonly ILogger<CachingInterceptor> _logger;
    private readonly ConcurrentDictionary<string, (object? Result, DateTime Expiry)> _cache = new();
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public CachingInterceptor(ILogger<CachingInterceptor> logger)
    {
        _logger = logger;
    }

    public async Task<object?> InterceptAsync(ExecutionContext context, CallHandler next)
    {
        var httpContext = context.HttpContext;

        // Only cache GET requests
        if (httpContext.Request.Method != "GET")
        {
            return await next.HandleAsync();
        }

        var cacheKey = GenerateCacheKey(context);

        // Check cache
        if (_cache.TryGetValue(cacheKey, out var cached) && cached.Expiry > DateTime.UtcNow)
        {
            _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return cached.Result;
        }

        // Execute and cache result
        var result = await next.HandleAsync();

        _cache[cacheKey] = (result, DateTime.UtcNow.Add(_cacheExpiration));
        _logger.LogInformation("Cached result for {CacheKey}", cacheKey);

        // Clean up expired entries
        CleanupExpiredEntries();

        return result;
    }

    private string GenerateCacheKey(ExecutionContext context)
    {
        var path = context.HttpContext.Request.Path.Value ?? string.Empty;
        var query = context.HttpContext.Request.QueryString.Value ?? string.Empty;
        return $"{path}{query}";
    }

    private void CleanupExpiredEntries()
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _cache
            .Where(kvp => kvp.Value.Expiry <= now)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _cache.TryRemove(key, out _);
        }
    }
}
