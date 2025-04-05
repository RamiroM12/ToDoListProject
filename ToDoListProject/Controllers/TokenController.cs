using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoListProject.Dtos;
using ToDoListProject.Interfaces;

namespace ToDoListProject.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly IAuthService _authService;

        public TokenController(IAuthService service) => _authService = service;

        [HttpPost("refresh")]     
        public async Task<IActionResult> Refresh()
        {
            HttpContext.Request.Cookies.TryGetValue("accesToken", out var accesToken);
            HttpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken);

            var tokenDto = new TokenDto(accesToken, refreshToken);

            var tokenToReturn = await _authService.RefreshToken(tokenDto);

            _authService.SetTokensInsideCookie(tokenToReturn, HttpContext);

            return Ok();

        }


        
    }
}
