using ModularNet.Core.Exceptions;
using ModularNet.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ModularNet.Core.Pipes;

public class ValidationPipe : IPipeTransform
{
    public async Task<object?> TransformAsync(object? value, Type targetType)
    {
        if (value == null)
        {
            return null;
        }

        var validationContext = new ValidationContext(value);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(value, validationContext, validationResults, true);

        if (!isValid)
        {
            var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
            throw new BadRequestException($"Validation failed: {errors}");
        }

        return value;
    }
}
