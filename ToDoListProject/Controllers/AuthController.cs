using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoListProject.Dtos.UserDtos;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;
using ToDoListProject.Services;

namespace ToDoListProject.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        [HttpGet("test-user-manager")]
        public IActionResult TestUserManager()
        {
            if (_userManager == null)
                return BadRequest("UserManager no está inyectado");

            return Ok("UserManager funciona correctamente");
        }

        public AuthController(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager, IAuthService authService, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("login-oauth")]
        public IActionResult LoginWithGoogle()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(LoginCallback)) 
            };
            return Challenge(props, "Google"); 
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> LoginCallback()
        {
           var results = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!results.Succeeded)
                return BadRequest("Error al autenticar con el proveedor externo");

            var validation = await _authService.validateUserFromOauth(results.Principal);

            //var validation = await _authService.validateUserFromOauth(User);

            if (validation == true)
           {
                var tokenDto = await _authService.GenerateToken(populateExp: true);

                _authService.SetTokensInsideCookie(tokenDto, HttpContext);

                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                return Redirect("/swagger");
           }

            return BadRequest("Error al autenticar con el proveedor externo");


        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = await _authService.RegisterUser(dto);
                               
                if (user == null)
                {
                    return BadRequest("Fallo al registrarse.");
                }               

                return Created("", "Usuario Registrado");
            }

            return BadRequest(ModelState);

        }

        [HttpPost("login")]
         public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
         {
            if (ModelState.IsValid)
            {
                var result = await _authService.ValidateUser(dto);

                if (result)
                {
                    var tokenDto = await _authService.GenerateToken(populateExp: true);

                    _authService.SetTokensInsideCookie(tokenDto, HttpContext);

                    return Ok();
                }

                return Unauthorized("Email o Contraseña Incorrectos.");


            }
             return BadRequest(ModelState);
         }


        [HttpGet("getUsers")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {     
            var users = await _userManager.Users.ToListAsync();

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Fullname = user.FullName,
                Email = user.Email,
                Username = user.UserName,
            });

            return Ok(userDtos);
        }
    }
}
