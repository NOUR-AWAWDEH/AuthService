using AuthService.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AuthService.Entity
{

    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required, MinLength(4), MaxLength(15)]
        public string UserName { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
        
        public UserRole Role { get; set; } = UserRole.User; // Use Enum for better security

        public string Email { get; set; } = string.Empty;

        public User()
        {
            UserId = Guid.NewGuid(); // Generate ID in constructor to avoid EF conflicts
        }

        public string? RefreshToken { get;set; } 
        
        public DateTime? RefreshTokenExpiryTime { get; set; }
        
    }
}