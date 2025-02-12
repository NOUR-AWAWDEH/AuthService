using AuthService.Entity;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthService.Models.Enums;
using AuthService.Models.Dtos;
namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthenService authService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return BadRequest("Username already exixts.");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Invalid username or password.");
            }
            return Ok(result);
        }

        [HttpPost("Refresh-Token")]
        public async Task<ActionResult> RefreshToken(RefreshTokenRequestDto request) 
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Incalid refresh token.");
            return Ok(result);
        }
        
        [Authorize]
        [HttpGet]
        public IActionResult AuthenticationOnlyEndpoint()
        {
            return Ok("You are Authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Admin-Only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an Admin");
        }
        
        //[HttpPost("ForgetPassword")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ForgetPassword(RequestForgetPasswordDto request)
        //{
        //    if (ModelState.IsValid) 
        //    {
        //        //valedate user
        //        var user = await authService.FindByEmailAsync(request.Email);
        //        if (User == null)
        //            return BadRequest("Invalid payload");

        //        //var token = await authService.GeneratePasswordResetTokenAsync(user);
        //        if (string.IsNullOrEmpty(token))
        //            return BadRequest("Something went wrong");

        //    }

        //    return BadRequest("");

        //}

    }
}
