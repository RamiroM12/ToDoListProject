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
    }
}
