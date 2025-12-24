using Microsoft.EntityFrameworkCore;
using WealthManager.Modules.Identity.Infrastructure.Database;

namespace WealthManager.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        // Find all types inheriting from DbContext in assemblies starting with "WealthManager"
        var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName != null && a.FullName.StartsWith("WealthManager"))
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsInterface && t != typeof(DbContext));

        foreach (var dbContextType in dbContextTypes)
        {
            try
            {
                // Resolve the DbContext from the container
                if (scope.ServiceProvider.GetService(dbContextType) is DbContext dbContext)
                {
                    Console.WriteLine($"Applying migrations for {dbContextType.Name}...");
                    dbContext.Database.Migrate();
                    Console.WriteLine($"Migrations applied for {dbContextType.Name}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations for {dbContextType.Name}: {ex.Message}");
                // Depending on requirement, we might want to throw to stop startup, or log and continue.
                // For now, logging to console is sufficient for dev.
            }
        }

        try
        {
            IdentityDataSeeder.SeedRolesAndAdminAsync(scope.ServiceProvider).Wait();
            Console.WriteLine("Identity Data Seeded Successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding data: {ex.Message}");
        }
    }
}
