using ToDoListProject.Dtos;
using ToDoListProject.Interfaces;

namespace ToDoListProject.Services
{
    public class CookieService : ICookieService
    {
        public CookieService() { }
        public void SetTokensInsideCookie(TokenDto TokenDto, HttpContext context)
        {
            context.Response.Cookies.Append("accesToken", TokenDto.AccesToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddHours(2),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

            context.Response.Cookies.Append("refreshToken", TokenDto.RefreshToken,
               new CookieOptions
               {
                   Expires = DateTimeOffset.UtcNow.AddDays(7),
                   HttpOnly = true,
                   IsEssential = true,
                   Secure = true,
                   SameSite = SameSiteMode.None
               });
        }
    }
}
