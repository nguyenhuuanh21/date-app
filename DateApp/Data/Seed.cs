using DateApp.DTOs;
using DateApp.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.Json;

namespace DateApp.Data
{
    public class Seed
    {
        public static async Task SeedUsers(AppDbContext context)
        {
            if(await context.AppUsers.AnyAsync()) return;
            var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);
            if(members == null)
            {
                Console.WriteLine("No members found in the seed data.");
                return;
            }
            using var hmac = new HMACSHA512();
            foreach(var member in members)
            {
                var user = new AppUser
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    Email = member.Email,
                    ImageUrl = member.ImageUrl,
                    PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("password")),
                    PasswordSalt = hmac.Key,
                    Member=new Member
                    {
                        Id = member.Id,
                        DisplayName = member.DisplayName,
                        DateOfBirth=member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        Created=member.Created,
                        LastActive=member.LastActive,
                        Gender=member.Gender,
                        Description=member.Description,
                        City=member.City,
                        Country=member.Country
                    }
                };
                user.Member.Photos.Add(new Photo
                {
                    Url = member.ImageUrl!,
                    MemberId = member.Id
                });
                context.Add(user);
            }
            await context.SaveChangesAsync();
        }
    }
}
