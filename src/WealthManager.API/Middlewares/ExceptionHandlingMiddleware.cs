using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using WealthManager.Shared.Exceptions;

namespace WealthManager.API.Middlewares;


public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Get correlation ID from RequestLoggingMiddleware
            var correlationId = context.Items["CorrelationId"]?.ToString()
                                ?? Activity.Current?.Id
                                ?? context.TraceIdentifier;

            // Generate unique error ID for user reference
            var errorId = GenerateErrorId();

            // Get user context for logging
            var userInfo = GetUserInfo(context);
            var endpoint = $"{context.Request.Method} {context.Request.Path}";

            // Log with full context
            _logger.LogError(ex,
                "[{CorrelationId}][{ErrorId}] {ExceptionType}: {Message} | User: {User} | Endpoint: {Endpoint}",
                correlationId,
                errorId,
                ex.GetType().Name,
                ex.Message,
                userInfo,
                endpoint
            );

            await HandleExceptionAsync(context, ex, correlationId, errorId);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string correlationId, string errorId)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            InternalServerException => StatusCodes.Status500InternalServerError,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized, // Keep for backward compatibility
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Title = exception switch
            {
                BadRequestException => "Bad Request",
                UnauthorizedException => "Unauthorized",
                ForbiddenException => "Forbidden",
                NotFoundException => "Not Found",
                ConflictException => "Conflict",
                ValidationException => "Validation Failed",
                InternalServerException => "Internal Server Error",
                UnauthorizedAccessException => "Unauthorized",
                _ => "An error occurred while processing your request."
            },
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Detail = _env.IsDevelopment() ? exception.Message : GetProductionMessage(exception)
        };

        // Add error tracking information (ALWAYS include, even in production)
        problemDetails.Extensions["errorId"] = errorId;
        problemDetails.Extensions["traceId"] = correlationId;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("O"); // ISO 8601 format

        // Add validation errors if it's a ValidationException
        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        if (_env.IsDevelopment())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["exceptionType"] = exception.GetType().FullName;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var problemDetailsJson = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await context.Response.WriteAsync(problemDetailsJson);
    }

    /// <summary>
    /// Get production-safe error message. Hides system error details from clients.
    /// </summary>
    private static string GetProductionMessage(Exception exception)
    {
        return exception switch
        {
            // Client errors - safe to show detailed message
            BadRequestException or ValidationException or NotFoundException => exception.Message,
            UnauthorizedException or ForbiddenException => exception.Message,
            ConflictException => exception.Message,

            // System errors - hide details in production
            _ => "An internal server error occurred. Please try again later."
        };
    }

    /// <summary>
    /// Generate unique error ID for user reference
    /// Format: ERR-{first 12 chars of guid}
    /// </summary>
    private static string GenerateErrorId()
    {
        return $"ERR-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
    }

    /// <summary>
    /// Extract user information from claims for logging
    /// </summary>
    private string GetUserInfo(HttpContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
            return "Anonymous";

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? context.User.FindFirst("sub")?.Value
                    ?? "Unknown";

        var email = context.User.FindFirst(ClaimTypes.Email)?.Value
                    ?? context.User.FindFirst("email")?.Value
                    ?? "";

        return email != "" ? $"{email} ({userId})" : $"User:{userId}";
    }
}