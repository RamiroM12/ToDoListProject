using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Dtos.UserDtos
{
    public class RegisterUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
