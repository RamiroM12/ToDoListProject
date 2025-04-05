using Microsoft.AspNetCore.Mvc;
using ToDoListProject.Dtos.TasksDtos;
using ToDoListProject.Models;

namespace ToDoListProject.Interfaces
{
    public interface ITaskService
    {
        Task<bool> PostTask(PostTaskDto dto, string id);

        Task<List<TaskDto>> GetUserDtoTasks(string id);

        Task<bool> DeleteTask(string userId, int taskId);

        Task<bool> UpdateTask(UpdateTaskDto dto, string userId);

        Task<List<TaskDto>> SortTasksByPriority(string id);
    }
}
