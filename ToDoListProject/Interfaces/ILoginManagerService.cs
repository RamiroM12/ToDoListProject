using System.Security.Claims;
using ToDoListProject.Dtos.UserDtos;
using ToDoListProject.Models;

namespace ToDoListProject.Interfaces
{
    public interface ILoginManagerService
    {
        public Task<bool> LoginManualAsync(LoginUserDto loginDto, HttpContext context);

        public Task<User?> RegisterUserAsync(RegisterUserDto registerDto, HttpContext context);

        public Task<bool> LoginOauthAsync (ClaimsPrincipal principal, HttpContext context);
    }
}
