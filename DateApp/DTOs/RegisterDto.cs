using System.ComponentModel.DataAnnotations;

namespace DateApp.DTOs
{
    public class RegisterDto
    {
        [Required]
        public  string DisplayName { get; set; }="";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        [MinLength(3)]
        public  string Password { get; set; }="";
        [Required]
        public string Gender { get; set; } = String.Empty;
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public string City { get; set; } = String.Empty;
        [Required]
        public string Country { get; set; } = String.Empty;
    }
}
