using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ToDoListProject.Models
{
    public class User : IdentityUser
    {
        [Required]
        public required string FullName { get; set; }

        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
