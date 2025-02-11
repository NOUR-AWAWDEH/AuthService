using AuthService.Models.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class UserDto
    {
        [Required]
        [JsonProperty(Order = 1)]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters long.")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [JsonProperty(Order = 2)]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonProperty(Order = 3)]
        public UserRole Role { get; set; } = UserRole.User; // Using an Enum prevents role injection

        [Required]
        [JsonProperty(Order = 4)]  
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;

    }

    
}
