using ToDoListProject.Models;

namespace ToDoListProject.Dtos.TasksDtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;

        public PriorityType Priority { get; set; }
    }
}
