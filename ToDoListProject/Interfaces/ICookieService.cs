using ToDoListProject.Dtos;

namespace ToDoListProject.Interfaces
{
    public interface ICookieService
    {
        public void SetTokensInsideCookie(TokenDto TokenDto, HttpContext context);
    }
}
