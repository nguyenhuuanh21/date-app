using DateApp.Entities;

namespace DateApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
