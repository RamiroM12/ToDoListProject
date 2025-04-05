using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public AuthController(IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager, IAuthService authService, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _logger = logger;
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

        [HttpPost("autoLogin")]
        public async Task<IActionResult> AutoLogin()
        {
            var loginDto = new LoginUserDto() {Email = "god123@gmail.com", Password = "God123_" };
            var result = await _authService.ValidateUser(loginDto);

            if (result)
            {
                var tokenDto = await _authService.GenerateToken(populateExp: true);

                _authService.SetTokensInsideCookie(tokenDto, HttpContext);

                return Ok();
            }

            return Unauthorized("Email o Contraseña Incorrectos.");
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
