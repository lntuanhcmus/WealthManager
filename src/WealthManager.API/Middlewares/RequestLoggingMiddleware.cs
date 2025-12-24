using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using WealthManager.Shared.Utilities;

namespace WealthManager.API.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    private const int MaxRequestBodySize = 10 * 1024;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Activity.Current?.Id ?? context.TraceIdentifier;
        context.Items["CorrelationId"] = correlationId;

        var userInfo = GetUserInfo(context);
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var requestBody = await ReadRequestBodyAsync(context.Request);

        // ONLY log if there's a body (avoid duplicate for simple GET requests)
        if (requestBody != "[Empty]" && requestBody != "[Unknown content type]")
        {
            _logger.LogInformation(
                "[{CorrelationId}] Request Body: {RequestBody} | User: {User} | IP: {IpAddress}",
                correlationId,
                requestBody,
                userInfo,
                ipAddress
            );
        }

        // Don't log "Processing" and "Completed" - let Serilog handle that
        await _next(context);
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength == 0) return "[Empty]";

        if (request.ContentLength > MaxRequestBodySize)
            return $"[Body too large: {request.ContentLength} bytes]";

        if (!request.ContentType?.Contains("application/json") ?? true)
            return $"[{request.ContentType ?? "Unknown content type"}]";

        try
        {
            // Enable buffering để có thể read nhiều lần
            request.EnableBuffering();

            // Read body
            using var reader = new StreamReader(
                request.Body,
                Encoding.UTF8,
                leaveOpen: true  // Important: Không close stream
            );

            var body = await reader.ReadToEndAsync();

            // Reset stream position về đầu cho controller đọc
            request.Body.Position = 0;

            // Mask sensitive data trước khi log
            return SensitiveDataMasker.MaskSensitiveData(body);
        }
        catch (Exception ex)
        {
            return $"[Error reading body: {ex.Message}]";
        }
    }

    private string GetUserInfo(HttpContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
            return "Anonymous";

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? context.User.FindFirst("sub")?.Value  // JWT standard claim
                    ?? "Unknown";

        var email = context.User.FindFirst(ClaimTypes.Email)?.Value
                    ?? context.User.FindFirst("email")?.Value
                    ?? "";

        var roles = context.User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var rolesString = roles.Any() ? $"[{string.Join(", ", roles)}]" : "";

        return email != ""
            ? $"{email} ({userId}) {rolesString}"
            : $"User:{userId} {rolesString}";
    }

}