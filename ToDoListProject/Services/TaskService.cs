using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListProject.Data;
using ToDoListProject.Dtos.TasksDtos;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;

namespace ToDoListProject.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;

        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PostTask(PostTaskDto dto, string id)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Priority = dto.Priority,
                UserId = id
            };

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<TaskDto>> GetUserDtoTasks(string id)
        {
            var userTasks = await _context.Tasks
                .Where(task => task.UserId == id)
                .Select(task => new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Priority = task.Priority,
                    IsCompleted = task.IsCompleted,
                    CreatedDate = task.CreatedDate,
                    LastUpdateDate = task.LastUpdateDate
                })
                .ToListAsync();

            return userTasks;
        }

        public async Task<bool> DeleteTask(string UserId, int taskId)
        {
            var userTasks = await GetUserTasks(UserId);

            var taskToDelete = userTasks.Find(task => task.Id == taskId);

            if (taskToDelete == null) return false;

            _context.Tasks.Remove(taskToDelete);
            _context.SaveChanges();

            return true;
        }

        private async Task<List<TaskItem>> GetUserTasks(string id)
        {
            var userTasks = await _context.Tasks
                .Where(task => task.UserId == id)                
                .ToListAsync();

            return userTasks;
        }

        public async Task<bool> UpdateTask(UpdateTaskDto dto ,string userId)
        {
            var userTask = await GetUserTasks(userId);

            var taskToUpdate = userTask.Find(task => task.Id == dto.Id);

            if (taskToUpdate == null) return false;

            taskToUpdate.Title = dto.Title;
            taskToUpdate.LastUpdateDate = DateTime.UtcNow;
            taskToUpdate.Priority = dto.Priority;
            taskToUpdate.IsCompleted = dto.IsCompleted;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TaskDto>> SortTasksByPriority(string id)
        {
            var taskList = await _context.Tasks
                .Where(task => task.UserId == id)
                .OrderByDescending(task => task.Priority)
                 .Select(task => new TaskDto
                 {
                     Id = task.Id,
                     Title = task.Title,
                     Priority = task.Priority,
                     IsCompleted = task.IsCompleted,
                     CreatedDate = task.CreatedDate,
                     LastUpdateDate = task.LastUpdateDate
                 })
                .ToListAsync();

            return taskList;
        }
    }
}


