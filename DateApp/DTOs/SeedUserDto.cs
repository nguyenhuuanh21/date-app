namespace DateApp.DTOs
{
    public class SeedUserDto
    {
        public required string Id { get; set; }
        public required string Email { get; set; } 
        public required DateOnly DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public required string DisplayName { get; set; } = null!;
        public DateTime Created { get; set; } 
        public DateTime LastActive { get; set; } 
        public required string? Gender { get; set; }
        public string? Description { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
    }
}
