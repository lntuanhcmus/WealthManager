using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WealthManager.Modules.Identity.Application.DTOs;
using WealthManager.Modules.Identity.Application.Interfaces;
using WealthManager.Modules.Identity.Domain.Constants;
using WealthManager.Modules.Identity.Domain.Entities;

namespace WealthManager.Modules.Identity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class RoleController : ControllerBase
{

    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _roleService.GetRolesAsync();
        return Ok(roles);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
    {
        var roleId = await _roleService.CreateRoleAsync(roleDto.RoleName);
        return Ok(new { RoleId = roleId });
    }
}