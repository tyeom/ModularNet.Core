namespace ModularNet.Core.Interfaces;

public interface IPipeTransform
{
    Task<object?> TransformAsync(object? value, Type targetType);
}
