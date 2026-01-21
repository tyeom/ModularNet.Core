using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModularNet.Core.Attributes;
using ModularNet.Core.Interfaces;
using System.Reflection;
using System.Text.Json;

namespace ModularNet.Core.Binding;

public static class ParameterBinder
{
    public static async Task<object?[]> BindParametersAsync(
        HttpContext httpContext,
        MethodInfo method,
        IServiceProvider serviceProvider)
    {
        var parameters = method.GetParameters();
        var boundValues = new object?[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            // Get raw value from request
            var rawValue = await GetRawValueAsync(httpContext, parameter);

            // Apply pipes in order
            var pipeAttributes = parameter.GetCustomAttributes<PipeAttribute>().ToArray();
            var transformedValue = rawValue;

            foreach (var pipeAttr in pipeAttributes)
            {
                var pipe = CreatePipeInstance(pipeAttr, serviceProvider);
                transformedValue = await pipe.TransformAsync(transformedValue, parameter.ParameterType);
            }

            boundValues[i] = transformedValue;
        }

        return boundValues;
    }

    private static async Task<object?> GetRawValueAsync(HttpContext context, ParameterInfo parameter)
    {
        // Try route values first
        if (context.Request.RouteValues.TryGetValue(parameter.Name!, out var routeValue))
        {
            return routeValue?.ToString();
        }

        // Try query string
        if (context.Request.Query.TryGetValue(parameter.Name!, out var queryValue))
        {
            return queryValue.ToString();
        }

        // Try reading from body for complex types
        if (!parameter.ParameterType.IsPrimitive &&
            parameter.ParameterType != typeof(string) &&
            parameter.ParameterType != typeof(int) &&
            parameter.ParameterType != typeof(long) &&
            parameter.ParameterType != typeof(double) &&
            parameter.ParameterType != typeof(bool) &&
            parameter.ParameterType != typeof(decimal))
        {
            return await ReadFromBodyAsync(context, parameter.ParameterType);
        }

        return null;
    }

    private static async Task<object?> ReadFromBodyAsync(HttpContext context, Type targetType)
    {
        try
        {
            context.Request.EnableBuffering();
            context.Request.Body.Position = 0;

            var result = await JsonSerializer.DeserializeAsync(
                context.Request.Body,
                targetType,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            context.Request.Body.Position = 0;
            return result;
        }
        catch
        {
            return null;
        }
    }

    private static IPipeTransform CreatePipeInstance(PipeAttribute pipeAttr, IServiceProvider serviceProvider)
    {
        // Try to get from DI first
        var pipeFromDI = serviceProvider.GetService(pipeAttr.PipeType) as IPipeTransform;
        if (pipeFromDI != null)
        {
            return pipeFromDI;
        }

        // Fall back to Activator with constructor args
        if (pipeAttr.ConstructorArgs.Length > 0)
        {
            return (IPipeTransform)Activator.CreateInstance(pipeAttr.PipeType, pipeAttr.ConstructorArgs)!;
        }

        // Fall back to default constructor
        return (IPipeTransform)Activator.CreateInstance(pipeAttr.PipeType)!;
    }
}
