using LogRegApp.Models;

namespace LogRegApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
