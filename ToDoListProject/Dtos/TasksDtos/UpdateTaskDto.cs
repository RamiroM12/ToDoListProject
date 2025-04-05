using ToDoListProject.Models;

namespace ToDoListProject.Dtos.TasksDtos
{
    public class UpdateTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public bool IsCompleted { get; set; } = false;

        public PriorityType Priority { get; set; }
    }
}
