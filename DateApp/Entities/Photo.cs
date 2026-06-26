using System.Text.Json.Serialization;

namespace DateApp.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public required string Url { get; set; } = null!;
        public string? PublicId { get; set; }
        //Navigation properties
        [JsonIgnore]
        public Member Member { get; set; } = null!;
        public string MemberId { get; set; } = null!;
    }
}
