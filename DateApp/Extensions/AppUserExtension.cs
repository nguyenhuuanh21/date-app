using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Interfaces;

namespace DateApp.Extensions
{
    public static class AppUserExtension
    {
        public static UserDto ToDto(this AppUser user,ITokenService tokenService)
        {
            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Id = user.Id,
                ImageUrl=user.ImageUrl,
                Token = tokenService.CreateToken(user)
            };
        }
    }
}
