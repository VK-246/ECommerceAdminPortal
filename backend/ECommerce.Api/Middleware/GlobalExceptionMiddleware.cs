using System.Text.Json;
using ECommerce.Shared.Exceptions;
using ECommerce.Shared.Responses;

namespace ECommerce.Api.Middleware;

/// <summary>
/// Global exception middleware — sits at the very top of the request pipeline.
/// Every unhandled exception bubbles up here. 
/// 1. We catch it once, 
/// 2. map it to the correct HTTP status code, 
/// 3. return a consistent ApiResponse JSON payload.
///
/// Without this: each controller action would need its own try-catch block.
/// With this: controllers stay clean — they just throw and this catches.
/// </summary>
public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass control to the next component in the pipeline
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the exception internally (full details for developers)
            logger.LogError(ex, "Unhandled exception caught by GlobalExceptionMiddleware: {Message}", ex.Message);

            // Map exception type → HTTP status code
            await HandleExceptionAsync(context, ex);
        }
    }

    // Cached once at class load time; reused for every serialization call.
    // JsonSerializerOptions is expensive to construct and internally caches
    // reflection/metadata per instance — throwing it away defeats that cache.
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Determine status code and message based on exception type
        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            BadRequestException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        context.Response.StatusCode = statusCode;

        // Build the standardised failure response
        var response = ApiResponse<object>.Fail(message);

        var json = JsonSerializer.Serialize(response, _jsonOptions);

        await context.Response.WriteAsync(json);
    }
}
