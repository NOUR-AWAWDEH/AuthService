using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Dtos
{
    public class RequestForgetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


    }
}
