using DateApp.Data;
using DateApp.DTOs;
using DateApp.Entities;
using DateApp.Extensions;
using DateApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DateApp.Controllers
{
    
    public class AccountController(AppDbContext context,ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await IsEmailExist(registerDto.Email))
            {
                return BadRequest("Email is already exist");
            }
            using var hmac=new HMACSHA3_512();
            var user=new AppUser
            {
                Email= registerDto.Email,
                DisplayName= registerDto.DisplayName,
                PasswordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };
            context.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
        private async Task<bool>IsEmailExist(string email)
        {
            return await context.AppUsers.AnyAsync(x=>x.Email.ToLower() == email.ToLower());
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody]LoginDto loginDto)
        {
            var user=await context.AppUsers.SingleOrDefaultAsync(x=>x.Email.ToLower() == loginDto.Email.ToLower());
            if(user == null)
            {
                return Unauthorized("Invalid email");
            }
            using var hmac=new HMACSHA3_512(user.PasswordSalt);
            var computedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDto.Password));

            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password");
                }
            }
            return user.ToDto(tokenService);
        }
    }
}
