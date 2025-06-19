using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToDoListProject.Dtos;
using ToDoListProject.Dtos.UserDtos;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;

namespace ToDoListProject.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _Configuration;
        private readonly UserManager<User> _UserManager;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, UserManager<User> userManager, ILogger<AuthService> logger)
        {
            _Configuration = configuration;
            _UserManager = userManager;
            _logger = logger;
        }

        async public Task<IdentityUser> RegisterUser(RegisterUserDto dto)
        {
            var existingUser = await _UserManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return null;
            }

            var user = new User() 
            {   Email = dto.Email, 
                FullName = dto.FullName, 
                UserName = dto.Username 
            };

            var result = await _UserManager.CreateAsync(user, dto.Password);

            return result.Succeeded ? user : null;

        }

        public async Task<bool> ValidateUser(LoginUserDto dto)
        {
            var findUser = await _UserManager.FindByEmailAsync(dto.Email);

            if (findUser != null && await _UserManager.CheckPasswordAsync(findUser, dto.Password))
            {
                return true;
            }

            return false;
        }

    }
}
