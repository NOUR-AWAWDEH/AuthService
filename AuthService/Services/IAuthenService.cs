using AuthService.Entity;
using AuthService.Models;

namespace AuthService.Services
{
    public interface IAuthenService
    {
        Task<User?> RegisterAsync(UserDto request);
        
        Task<TokenResponseDto?> LoginAsync(LoginDto request);
        
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
