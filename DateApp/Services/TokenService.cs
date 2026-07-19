using DateApp.Entities;
using DateApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace DateApp.Services
{
    public class TokenService(IConfiguration configuration,
        UserManager<AppUser> userManager) : ITokenService
    {
        public async Task<string> CreateToken(AppUser user)
        {
            var tokenKey= configuration["TokenKey"] ?? throw new Exception("TokenKey is not configured.");
            if(tokenKey.Length<64) throw new Exception("TokenKey must be at least 64 characters long.");
            var key=new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenKey));
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Email,user.Email!),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
            };
            var roles=await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r=>new Claim(ClaimTypes.Role,r)));
            var creds=new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor=new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(7),
                SigningCredentials=creds
            };
            var tokenHandler=new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);


        }
        public string genarateRefreshToken()
        {
            var randomBytes=RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
