using System.Net;
using System.Text.Json;
using UserManagement.Domain.Exceptions;

namespace UserManagement.API.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        var (statusCode, message) = MapException(exception);

        if (statusCode == (int)HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "An unexpected error occurred.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new { error = message };
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static (int statusCode, string message) MapException(Exception exception) =>
        exception switch
        {
            NotFoundException ex       => ((int)HttpStatusCode.NotFound, ex.Message),
            DomainException ex         => ((int)HttpStatusCode.UnprocessableEntity, ex.Message),
            _                          => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
}

