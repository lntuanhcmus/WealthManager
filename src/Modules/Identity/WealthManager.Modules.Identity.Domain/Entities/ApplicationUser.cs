using Microsoft.AspNetCore.Identity;

namespace WealthManager.Modules.Identity.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
