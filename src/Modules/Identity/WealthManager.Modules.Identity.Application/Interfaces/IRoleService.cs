using System.Collections.Generic;
using System.Threading.Tasks;

namespace WealthManager.Modules.Identity.Application.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<string>> GetRolesAsync();
    Task<string> CreateRoleAsync(string roleName);
}