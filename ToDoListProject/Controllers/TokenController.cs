using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListProject.Dtos;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;

namespace ToDoListProject.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        private readonly UserManager<User> _userManager;

        public TokenController(ITokenService tokenService, ICookieService cookieService, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _cookieService = cookieService;
            _userManager = userManager;
        }
        [HttpPost("refresh")]     
        public async Task<IActionResult> Refresh()
        {
            HttpContext.Request.Cookies.TryGetValue("accesToken", out var accesToken);
            HttpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken);

            var tokenDto = new TokenDto(accesToken, refreshToken);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var tokenToReturn = await _tokenService.RefreshToken(user, tokenDto);

            _cookieService.SetTokensInsideCookie(tokenToReturn, HttpContext);

            return Ok();

        }


        
    }
}
