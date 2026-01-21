using ModularNet.Core.Exceptions;
using ModularNet.Core.Interfaces;

namespace ModularNet.Core.Pipes;

public class ParseIntPipe : IPipeTransform
{
    private readonly int? _defaultValue;
    private readonly bool _allowNull;

    public ParseIntPipe() : this(null, false)
    {
    }

    public ParseIntPipe(int? defaultValue) : this(defaultValue, false) { }

    public ParseIntPipe(int? defaultValue = null, bool allowNull = false)
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
        if (int.TryParse(stringValue, out var result))
        {
            return Task.FromResult<object?>(result);
        }

        if (_defaultValue.HasValue)
        {
            return Task.FromResult<object?>(_defaultValue.Value);
        }

        throw new BadRequestException($"'{stringValue}' is not a valid integer");
    }
}
