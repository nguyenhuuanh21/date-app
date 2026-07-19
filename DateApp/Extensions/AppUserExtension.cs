using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Interfaces;
using System.Threading.Tasks;

namespace DateApp.Extensions
{
    public static class AppUserExtension
    {
        public static async Task<UserDto> ToDto(this AppUser user,ITokenService tokenService)
        {
            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Id = user.Id,
                ImageUrl=user.ImageUrl,
                Token = await tokenService.CreateToken(user)
            };
        }
    }
}
