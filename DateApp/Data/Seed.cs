using DateApp.DTOs;
using DateApp.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.Json;

namespace DateApp.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;
            var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);
            if (members == null)
            {
                Console.WriteLine("No members found in the seed data.");
                return;
            }
            foreach (var member in members)
            {
                var user = new AppUser
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    UserName = member.Email,
                    Email = member.Email,
                    ImageUrl = member.ImageUrl,
                    Member = new Member
                    {
                        Id = member.Id,
                        DisplayName = member.DisplayName,
                        DateOfBirth = member.DateOfBirth,
                        ImageUrl = member.ImageUrl,
                        Created = member.Created,
                        LastActive = member.LastActive,
                        Gender = member.Gender,
                        Description = member.Description,
                        City = member.City,
                        Country = member.Country
                    }
                };
                user.Member.Photos.Add(new Photo
                {
                    Url = member.ImageUrl!,
                    MemberId = member.Id
                });
                var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                await userManager.AddToRoleAsync(user, "Member");
            }
            var admin = new AppUser
            {
                DisplayName = "Admin",
                UserName = "admin",
                Email = "admin@gmail.com"
            };
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, ["Admin", "Moderator" ]);
        }
    }
}
