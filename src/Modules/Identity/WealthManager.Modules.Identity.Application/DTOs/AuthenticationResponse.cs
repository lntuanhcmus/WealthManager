using WealthManager.Modules.Identity.Domain.Entities;

namespace WealthManager.Modules.Identity.Application.DTOs;
public class AuthenticationResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsVerified { get; set; }
    public string JwToken { get; set; } = string.Empty;      // Access Token
    public RefreshToken RefreshToken { get; set; } = null!; // Refresh Token
}