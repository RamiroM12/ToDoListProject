using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Dtos.UserDtos
{
    public class UserDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
    }
}
