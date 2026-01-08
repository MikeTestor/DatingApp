using DatingApp.API.DTOs;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;

namespace DatingApp.API.Extensions;

public static class AppUserExtension
{
    public static UserDto ToUserDto(this AppUser user, ITokenService tokenService)
    {
        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };
    }
}
