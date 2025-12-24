using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WealthManager.Modules.Identity.Application.Interfaces;

namespace WealthManager.Modules.Identity.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleService(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<string>> GetRolesAsync()
    {
        return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
    }

    public async Task<string> CreateRoleAsync(string roleName)
    {
        var role = new IdentityRole { Name = roleName };
        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).First();
            throw new Exception($"Failed to create role: {errors}");
        }

        return role.Id;
    }
}