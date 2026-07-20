using DateApp.Data;
using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DateApp.Controllers
{
    
    public class AccountController(UserManager<AppUser> userManager ,
        ITokenService tokenService
        ) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            
            var user = new AppUser { 
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName=registerDto.Email,
                Member = new Member
                {
                    DateOfBirth = registerDto.DateOfBirth,
                    DisplayName = registerDto.DisplayName,
                    Gender = registerDto.Gender,
                    City = registerDto.City,    
                    Country= registerDto.Country,
                }
            };
            var result=await userManager.CreateAsync(user, registerDto.Password);
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem(ModelState);
            }
            var roleResult = await userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return ValidationProblem(ModelState);
            }
            return await user.ToDto(tokenService);
        }
        //private async Task<bool>IsEmailExist(string email)
        //{
        //    return await context.Users.AnyAsync(x=>x.Email!.ToLower() == email.ToLower());
        //}
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody]LoginDto loginDto)
        {
            var user=await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email");
            }

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if(!result)
            {
                return Unauthorized("Invalid password");
            }
            await SetRefreshTokenCookie(user);
            return await user.ToDto(tokenService);
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh token is missing");
            }
            var user = await userManager.Users.FirstOrDefaultAsync(x=>x.RefreshToken== refreshToken&&x.RefreshTokenExpiryTime>DateTime.UtcNow);
            if (user == null)
            {
                return Unauthorized("Invalid or expired refresh token");
            }
            await SetRefreshTokenCookie(user);
            return await user.ToDto(tokenService);
        }
        private async Task SetRefreshTokenCookie(AppUser user)
        {
            var refreshToken = tokenService.genarateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
