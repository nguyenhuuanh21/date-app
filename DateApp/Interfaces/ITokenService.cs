using DateApp.Entities;

namespace DateApp.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
        string genarateRefreshToken();
    }
}
