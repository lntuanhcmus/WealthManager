using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using WealthManager.Modules.Identity.Domain.Entities;
using WealthManager.Modules.Identity.Domain.Constants;
namespace WealthManager.Modules.Identity.Infrastructure.Database;

public static class IdentityDataSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        //1. Create scope to get RoleManager & UserManager
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        //2. Seed Roles (Admin, User)
        string[] roleNames = { Roles.Admin, Roles.User };
        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        //3. Seed Admin User
        var adminEmail = "admin@wealth.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create admin user");
            }
            await userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }
    }
}