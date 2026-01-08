using System;
using DatingApp.API.Models;

namespace DatingApp.API.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}
