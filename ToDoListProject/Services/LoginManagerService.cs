using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Security.Claims;
using ToDoListProject.Dtos.UserDtos;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;

namespace ToDoListProject.Services
{
    public class LoginManagerService : ILoginManagerService
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        private readonly IOauthService _oauthService;
        private readonly UserManager<User> _userManager;

        public LoginManagerService(
            IAuthService authService,
            ITokenService tokenService,
            ICookieService cookieService,
            IOauthService oauthService,
            UserManager<User> userManager) 
        {
            _authService = authService;
            _tokenService = tokenService;
            _cookieService = cookieService;
            _oauthService = oauthService;
            _userManager = userManager;
        }

        public async Task<User?> RegisterUserAsync(RegisterUserDto registerDto, HttpContext context)
        {
            var user = await _authService.RegisterUser(registerDto);
            if (user == null) return null;

            var token = await _tokenService.GenerateToken((User)user, true);
            _cookieService.SetTokensInsideCookie(token, context);

            return (User)user;
        }

        public async Task<bool> LoginManualAsync(LoginUserDto loginDto, HttpContext context)
        {
            var isValid = await _authService.ValidateUser(loginDto);
            if (!isValid) return false;

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            var token = await _tokenService.GenerateToken(user, true);
            _cookieService.SetTokensInsideCookie(token, context);

            return true;
        }

        public async Task<bool> LoginOauthAsync(ClaimsPrincipal principal, HttpContext context)
        {
            var user = await _oauthService.validateUserFromOauth(principal);

           

            if (user == null) return false;

            var token = await _tokenService.GenerateToken(user, true);
            _cookieService.SetTokensInsideCookie(token, context);

            return true;
        }

    }
}
