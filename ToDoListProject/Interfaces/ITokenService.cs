using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ToDoListProject.Dtos;
using ToDoListProject.Models;

namespace ToDoListProject.Interfaces
{
    public interface ITokenService
    {
        public Task<TokenDto> GenerateToken(User user, bool populateExp);

        public string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalsFromExpiredTokens(string token);

        public Task<User> GetUsersFromTokens(string accesToken);

        public Task<TokenDto> RefreshToken(User user, TokenDto dto);
        
    }
}
