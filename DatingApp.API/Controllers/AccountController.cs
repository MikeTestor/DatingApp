using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Extensions;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    public class AccountController (AppDbContext context) : ControllerApiBase
    {
        [HttpPost("register")]  // localhost:5001/api/account/register
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto, ITokenService tokenService)
        {            
            if (await UserExists(registerDto.Email))
                return BadRequest("Email is already taken");

            AppUser user = await CreateUser(registerDto.Email, registerDto.DisplayName, registerDto.Password);
            context.Add(user);
            await context.SaveChangesAsync();

            UserDto userDto = user.ToUserDto(tokenService);

            return Ok(userDto);
        }        
        private async Task<AppUser> CreateUser(string email, string displayName, string password)
        {
            var hmac = new HMACSHA512();        
            var user = new AppUser
            {
                Email = email.ToLower(),
                DisplayName = displayName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };                       
            return user;
        }               

        [HttpPost("login")]  // localhost:5001/api/account/login    
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto, ITokenService tokenService)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email.ToLower());
            if (user == null)
                return Unauthorized("Invalid email");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid password");
            }
            UserDto userDto = user.ToUserDto(tokenService);

            return Ok(userDto);
        }
        private async Task<bool> UserExists(string email)
        {
            return await context.Users.AnyAsync(u => u.Email == email.ToLower());
        }     
    }
}
