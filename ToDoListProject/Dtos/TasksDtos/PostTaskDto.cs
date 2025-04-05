using System.ComponentModel.DataAnnotations;
using ToDoListProject.Models;

namespace ToDoListProject.Dtos.TasksDtos
{
    public class PostTaskDto
    {
        [Required]
        public string Title { get; set; }
        
        public PriorityType Priority { get; set; }
     

    }
}
