using Microsoft.AspNetCore.Identity;

namespace ViaitaliaAPI.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
