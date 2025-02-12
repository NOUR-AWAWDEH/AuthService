using AuthService.Data;
using AuthService.Entity;
using AuthService.Models.Dtos;
using AuthService.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Services
{
    public class AuthenService(AppDbContext context, IConfiguration configuration) : IAuthenService
    {
        public async Task<TokenResponseDto?> LoginAsync(LoginDto request)
        {
            // Normalize input
            var normalizedUserName = request.Username.Trim().ToLower();

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == normalizedUserName);
            if (user is null)
            {
                return null;
            }

            // Verify password
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                    == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var response = new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user),
            };
            return response;
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto reques)
        {
            var user = await ValidateRefreshTokenAsync(reques.UserId, reques.RefreshToken);
            if (user is null)
                return null;

            return await CreateTokenResponse(user);
        }

        private async Task<User> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null
                || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);

        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Enum to string
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                    issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                    audience: configuration.GetValue<string>("AppSettings:Audience"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            // Normalize input
            var normalizedUserName = request.UserName.Trim().ToLower();
            var normalizedEmail = request.Email.Trim().ToLower();

            // Check if the username or email already exists
            if (await context.Users.AnyAsync(u => u.UserName == normalizedUserName || u.Email == normalizedEmail))
            {
                return null; // Don't expose whether email or username already exists
            }

            // Validate Role against enum
            if (!Enum.TryParse(request.Role.ToString(), true, out UserRole userRole))
            {
                userRole = UserRole.User; // Default to "User" role if invalid
            }

            // Create a new user
            var user = new User
            {
                UserName = normalizedUserName,
                Email = normalizedEmail,
                Role = userRole
            };

            // Hash the password
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            // Add the user to the database
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            // Find the user by email
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null; // User does not exist
            }

            // Generate a refresh token (used as a reset token here)
            var resetToken = GenerateRefreshToken();
            user.RefreshToken = resetToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

            await context.SaveChangesAsync(); // Save the token in the database

            return resetToken; // Return the generated token
        }

    }

}
