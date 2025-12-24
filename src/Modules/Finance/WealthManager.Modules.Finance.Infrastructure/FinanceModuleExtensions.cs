using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WealthManager.Modules.Finance.Infrastructure.Database;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WealthManager.Modules.Finance.Infrastructure;

public static class FinanceModuleExtensions
{
    public static IServiceCollection AddFinanceModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:FinanceDb"];

        services.AddDbContext<FinanceDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));

        return services;
    }

    public static IServiceCollection AddFinanceModuleHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<FinanceDbContext>(
                name: "finance-database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "finance", "ready" }
            );
        return services;
    }
}
