using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WealthManager.Modules.Finance.Infrastructure;
using WealthManager.Modules.Identity.Infrastructure;
namespace WealthManager.API.Extensions;
public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        // Module health checks
        services.AddIdentityModuleHealthChecks();
        services.AddFinanceModuleHealthChecks();
        // Application-level liveness check
        services.AddHealthChecks()
            .AddCheck("self", () =>
                HealthCheckResult.Healthy("API is running"),
                tags: new[] { "live" }
            );
        // Health Check UI Dashboard
        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(30);
            setup.AddHealthCheckEndpoint("WealthManager API", "/health");
        })
        .AddInMemoryStorage();
        return services;
    }

    public static void MapApplicationHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        // Kubernetes Liveness Probe
        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        // Kubernetes Readiness Probe
        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        // Detailed health report
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        // Health Check UI Dashboard
        endpoints.MapHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
        });
    }

}