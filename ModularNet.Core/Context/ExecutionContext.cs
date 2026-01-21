using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace ModularNet.Core.Context;

public class ExecutionContext
{
    public required HttpContext HttpContext { get; init; }
    public required Type ControllerType { get; init; }
    public required MethodInfo Method { get; init; }
    public required object?[] Arguments { get; init; }
    public IDictionary<string, object> Metadata { get; } = new Dictionary<string, object>();
}
