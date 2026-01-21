using ModularNet.Core.Context;

namespace ModularNet.Core.Interfaces;

public interface IInterceptor
{
    Task<object?> InterceptAsync(Context.ExecutionContext context, CallHandler next);
}
