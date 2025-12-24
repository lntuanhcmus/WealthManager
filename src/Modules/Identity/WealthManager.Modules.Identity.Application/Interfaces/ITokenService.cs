using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WealthManager.Modules.Identity.Domain.Entities;

namespace WealthManager.Modules.Identity.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);

        RefreshToken GenerateRefreshToken(string userId);
    }
}
