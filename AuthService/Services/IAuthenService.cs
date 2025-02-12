using AuthService.Entity;
using AuthService.Models.Dtos;

namespace AuthService.Services
{
    public interface IAuthenService
    {
        Task<User?> RegisterAsync(UserDto request);
        
        Task<TokenResponseDto?> LoginAsync(LoginDto request);
        
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
        
        Task<User?> FindByEmailAsync(string email);

        Task<string?> GeneratePasswordResetTokenAsync(string email);
    }
}
