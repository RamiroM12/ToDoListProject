using Microsoft.EntityFrameworkCore;

namespace ToDoListProject.Models
{
    public enum PriorityType : Byte
    {
        None,
        Low,
        Medium,
        High,
        Critical
    }
}