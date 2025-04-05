using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ToDoListProject.Dtos;
using ToDoListProject.Dtos.UserDtos;
using ToDoListProject.Models;

namespace ToDoListProject.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityUser> RegisterUser(RegisterUserDto dto);
        Task<bool> ValidateUser(LoginUserDto dto);
        Task<TokenDto> GenerateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto dto);
        void SetTokensInsideCookie(TokenDto TokenDto, HttpContext context);
        Task<User> getUserFromToken(string accesToken);
    }
}
