using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DateApp.Entities
{
    public class Member
    {
        public string Id { get; set; } = null!;
        public required DateOnly DateOfBirth { get; set; }
        public string? ImageUrl { get; set; } 
        public required string DisplayName { get; set; } = null!;
        public DateTime Created { get; set; }=DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public required string? Gender { get; set; }
        public string? Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        //Navigation properties
        [JsonIgnore]
        public List<Photo> Photos { get; set; } = [];

        [JsonIgnore]
        public List<MemberLike> LikedByMembers { get; set; } = [];

        [JsonIgnore]
        public List<MemberLike> LikedMembers { get; set; } = [];
        [JsonIgnore]
        public List<Message> MessagesSent { get; set; } = [];
        [JsonIgnore]
        public List<Message> MessagesReceived { get; set; } = [];
        [JsonIgnore]
        [ForeignKey(nameof(Id))]
        public AppUser AppUser { get; set; } = null!;

    }
}
