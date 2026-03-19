using System.Net;
using System.Text.Json;
using SmartCVAnalyzer.Domain.Exceptions;

namespace SmartCVAnalyzer.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string message;
        IDictionary<string, string[]>? errors = null;

        if (exception is NotFoundException notFound)
        {
            statusCode = HttpStatusCode.NotFound;
            message = notFound.Message;
        }
        else if (exception is Domain.Exceptions.ValidationException validation)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = validation.Message;
            errors = (IDictionary<string, string[]>?)(validation.Errors.Count > 0
                ? validation.Errors
                : null);
        }
        else if (exception is DomainException domain)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = domain.Message;
        }
        else
        {
            statusCode = HttpStatusCode.InternalServerError;
            message = "An unexpected error occurred. Please try again later.";
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse((int)statusCode, message, errors);

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public record ErrorResponse(
    int StatusCode,
    string Message,
    IDictionary<string, string[]>? Errors = null
);