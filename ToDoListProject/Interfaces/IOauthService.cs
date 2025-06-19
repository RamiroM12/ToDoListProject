using System.Security.Claims;
using ToDoListProject.Models;

namespace ToDoListProject.Interfaces
{
    public interface IOauthService
    {
        public Task<User> validateUserFromOauth(ClaimsPrincipal principals);
    }
}
