using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ModularNet.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace ModularNet.Core.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware>? _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware>? logger = null)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger?.LogError(exception, "An error occurred: {Message}", exception.Message);

        var statusCode = exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            NotFoundException => StatusCodes.Status404NotFound,
            HttpException httpEx => httpEx.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };

        var response = new
        {
            StatusCode = statusCode,
            Message = exception.Message,
            Type = exception.GetType().Name
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
