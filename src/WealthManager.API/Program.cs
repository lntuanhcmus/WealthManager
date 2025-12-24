using WealthManager.Modules.Finance.Infrastructure;
using WealthManager.Modules.Identity.Infrastructure;
using WealthManager.API.Extensions;
using WealthManager.API.Middlewares;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting WealthManager API");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.ConfigureSerilog();

    builder.Services.AddFinanceModule(builder.Configuration);
    builder.Services.AddIdentityModule(builder.Configuration);

    builder.Services.AddApplicationHealthChecks();


    builder.Services.AddControllers(); // Register controllers

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLoggingWithEnrichment();
    app.UseMiddleware<RequestLoggingMiddleware>();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers(); // To route requests to endpoints
    app.MapApplicationHealthChecks();

    app.ApplyMigrations();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}