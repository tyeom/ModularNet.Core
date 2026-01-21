using ModularNet.Core.Exceptions;
using ModularNet.Core.Interfaces;

namespace ModularNet.Core.Pipes;

public class ParseDoublePipe : IPipeTransform
{
    private readonly double? _defaultValue;
    private readonly bool _allowNull;

    public ParseDoublePipe() : this(null, false)
    {
    }

    public ParseDoublePipe(double? defaultValue) : this(defaultValue, false) { }

    public ParseDoublePipe(double? defaultValue = null, bool allowNull = false)
    {
        _defaultValue = defaultValue;
        _allowNull = allowNull;
    }

    public Task<object?> TransformAsync(object? value, Type targetType)
    {
        if (value == null)
        {
            if (_allowNull) return Task.FromResult<object?>(null);
            if (_defaultValue.HasValue) return Task.FromResult<object?>(_defaultValue.Value);
            throw new BadRequestException("Value cannot be null");
        }

        var stringValue = value.ToString();
        if (double.TryParse(stringValue, out var result))
        {
            return Task.FromResult<object?>(result);
        }

        if (_defaultValue.HasValue)
        {
            return Task.FromResult<object?>(_defaultValue.Value);
        }

        throw new BadRequestException($"'{stringValue}' is not a valid number");
    }
}
