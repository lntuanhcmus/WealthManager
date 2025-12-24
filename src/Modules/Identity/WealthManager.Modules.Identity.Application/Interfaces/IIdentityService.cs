using WealthManager.Modules.Identity.Application.DTOs;

namespace WealthManager.Modules.Identity.Application.Interfaces;

public interface IIdentityService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<AuthenticationResponse> LoginAsync(LoginRequest request);

    Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request);
}