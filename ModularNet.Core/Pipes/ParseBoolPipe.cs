using ModularNet.Core.Exceptions;
using ModularNet.Core.Interfaces;

namespace ModularNet.Core.Pipes;

public class ParseBoolPipe : IPipeTransform
{
    private readonly bool? _defaultValue;
    private readonly bool _allowNull;

    public ParseBoolPipe() : this(null, false)
    {
    }

    public ParseBoolPipe(bool? defaultValue) : this(defaultValue, false) { }

    public ParseBoolPipe(bool? defaultValue = null, bool allowNull = false)
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

        var stringValue = value.ToString()?.ToLowerInvariant();

        if (stringValue == "true" || stringValue == "1")
        {
            return Task.FromResult<object?>(true);
        }

        if (stringValue == "false" || stringValue == "0")
        {
            return Task.FromResult<object?>(false);
        }

        if (_defaultValue.HasValue)
        {
            return Task.FromResult<object?>(_defaultValue.Value);
        }

        throw new BadRequestException($"'{value}' is not a valid boolean");
    }
}
