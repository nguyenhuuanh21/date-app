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
        public  string Password { get; set; }
    }
}
