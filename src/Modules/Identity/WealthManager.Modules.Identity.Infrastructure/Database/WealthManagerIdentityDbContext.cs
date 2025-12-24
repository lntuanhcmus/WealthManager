using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WealthManager.Modules.Identity.Domain.Entities;

namespace WealthManager.Modules.Identity.Infrastructure.Database;

public class WealthManagerIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public WealthManagerIdentityDbContext(DbContextOptions<WealthManagerIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("identity");
    }
}
