using Microsoft.AspNetCore.Mvc;
using WealthManager.Modules.Identity.Application.Interfaces;
using WealthManager.Modules.Identity.Application.DTOs;

namespace WealthManager.Modules.Identity.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerDto)
    {
        var userId = await _identityService.RegisterAsync(registerDto);
        return Ok(new { UserId = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginDto)
    {
        var response = await _identityService.LoginAsync(loginDto);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenDto)
    {
        var response = await _identityService.RefreshTokenAsync(refreshTokenDto);
        return Ok(response);
    }
}