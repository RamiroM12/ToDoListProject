using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Dtos.UserDtos
{
    public class LoginUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
