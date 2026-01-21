namespace ModularNet.Core.Context;

public class CallHandler
{
    private readonly Func<Task<object?>> _handler;

    public CallHandler(Func<Task<object?>> handler)
    {
        _handler = handler;
    }

    public Task<object?> HandleAsync() => _handler();
}
