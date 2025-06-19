using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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
        private readonly ILoginManagerService _loginManager;
        private readonly ILogger<AuthController> _logger;

        [HttpGet("test-user-manager")]
        public IActionResult TestUserManager()
        {
            if (_userManager == null)
                return BadRequest("UserManager no está inyectado");

            return Ok("UserManager funciona correctamente");
        }

        public AuthController(
            IConfiguration configuration,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILoginManagerService loginManager,
            ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _loginManager = loginManager;
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
                return BadRequest("Error al autenticar con el proveedor externo :((");

            var principals = results.Principal;          

            var succeed = await _loginManager.LoginOauthAsync(principals, HttpContext);

            //return succeed ? Ok() : BadRequest("Error al autenticar con el proveedor externo");

            return Redirect("/swagger");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = await _loginManager.RegisterUserAsync(dto, HttpContext);
                               
                return user == null ? BadRequest("El usuario ya existe") : Ok();
            }

            return BadRequest(ModelState);

        }

        [HttpPost("login")]
         public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
         {
            if (ModelState.IsValid)
            {
                var result = await _loginManager.LoginManualAsync(dto, HttpContext);

                return result ? Ok() : Unauthorized();
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
