using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Security.Claims;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;

namespace ToDoListProject.Services
{
    public class OauthService : IOauthService
    {
        private readonly UserManager<User> _userManager;

        public OauthService(UserManager<User> userManager) 
        {
            _userManager = userManager;
        }
        

        public async Task<User> validateUserFromOauth(ClaimsPrincipal principals)
        {
            var email = principals?.FindFirst(ClaimTypes.Email)?.Value;
            var name = principals?.FindFirst(ClaimTypes.Name)?.Value;

            Debug.WriteLine($"Email: {email} --- name: {name}");

            if (email is null || name is null)
                return null;

            var username = email.Split("@")[0];

            var user = await _userManager.FindByEmailAsync(email);

            if (user != null) return user;

            user = new User
            {
                Email = email,
                FullName = name,
                UserName = username,
            };

            var result = await _userManager.CreateAsync(user);
            //return result.Succeeded ? user : null;
            if (result.Succeeded) return user;

            for (var i = 0; i < 10; i++)
            {
                Debug.WriteLine(result.Errors.ToString());
            }

            return null;
        }
    }
}
