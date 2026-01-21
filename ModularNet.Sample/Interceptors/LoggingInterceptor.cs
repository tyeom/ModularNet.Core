using ModularNet.Core.Attributes;
using ModularNet.Core.Context;
using ModularNet.Core.Enums;
using ModularNet.Core.Interfaces;
using ExecutionContext = ModularNet.Core.Context.ExecutionContext;

namespace ModularNet.Sample.Interceptors;

[Injectable(ServiceScope.Scoped)]
public class LoggingInterceptor : IInterceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public async Task<object?> InterceptAsync(ExecutionContext context, CallHandler next)
    {
        var methodName = $"{context.ControllerType.Name}.{context.Method.Name}";

        _logger.LogInformation("Before executing {MethodName}", methodName);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await next.HandleAsync();
            stopwatch.Stop();

            _logger.LogInformation("After executing {MethodName} - took {ElapsedMs}ms",
                methodName, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Error executing {MethodName} - took {ElapsedMs}ms",
                methodName, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
