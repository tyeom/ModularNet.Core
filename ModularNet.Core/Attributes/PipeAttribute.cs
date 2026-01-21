using ModularNet.Core.Interfaces;

namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
public class PipeAttribute : Attribute
{
    public Type PipeType { get; }
    public object?[] ConstructorArgs { get; }

    public PipeAttribute(Type pipeType, params object?[] constructorArgs)
    {
        if (!typeof(IPipeTransform).IsAssignableFrom(pipeType))
        {
            throw new ArgumentException($"{pipeType.Name} must implement IPipeTransform");
        }

        PipeType = pipeType;
        ConstructorArgs = constructorArgs;
    }
}
