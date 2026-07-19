using Microsoft.AspNetCore.Identity;

namespace DateApp.Entities
{
    public class AppUser:IdentityUser
    {
        public required string DisplayName { get; set; } 
        public string? ImageUrl { get; set; }
        public string?RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        //Navigation properties
        public Member Member { get; set; } = null!;

    }
}
