using ModularNet.Core.Interfaces;

namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class UseInterceptorsAttribute : Attribute
{
    public Type[] InterceptorTypes { get; }

    public UseInterceptorsAttribute(params Type[] interceptorTypes)
    {
        foreach (var type in interceptorTypes)
        {
            if (!typeof(IInterceptor).IsAssignableFrom(type))
            {
                throw new ArgumentException($"{type.Name} must implement IInterceptor");
            }
        }

        InterceptorTypes = interceptorTypes;
    }
}
