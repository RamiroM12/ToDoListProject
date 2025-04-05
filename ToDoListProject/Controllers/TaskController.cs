using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListProject.Dtos.TasksDtos;
using ToDoListProject.Interfaces;
using ToDoListProject.Models;

namespace ToDoListProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;

        public TaskController(ITaskService taskService, UserManager<User> userManager, IAuthService authService)
        {
            _taskService = taskService;
            _userManager = userManager;
            _authService = authService;
        }

        [HttpPost("postTask")]
        [Authorize]
        public async Task<IActionResult> PostTask([FromBody] PostTaskDto dto)
        {
            if (dto == null) return BadRequest("Los datos proporcionados son invalidos");

            HttpContext.Request.Cookies.TryGetValue("accesToken", out var accesToken);

            var user = await _authService.getUserFromToken(accesToken);

            var isCreated = await _taskService.PostTask(dto, user.Id.ToString() );

            if (isCreated)
                return StatusCode(201);

            return StatusCode(500, "Error al guardar la tarea. Inténtalo más tarde.");
        }

        [HttpGet("getTasks")]
        [Authorize]
        public async Task<IActionResult> GetUserTasks()
        {
            HttpContext.Request.Cookies.TryGetValue("accesToken", out var accesToken);

            var user = await _authService.getUserFromToken(accesToken);

            var userTasks = await _taskService.GetUserDtoTasks(user.Id);

            if (userTasks == null || !userTasks.Any())
                return BadRequest("Ninguna tarea encontrada");

            return Ok(userTasks);
        }

        [HttpDelete("deleteTaskById")]
        [Authorize]
        public async Task<IActionResult> DeleteUserTaskById(int id)
        {
            HttpContext.Request.Cookies.TryGetValue("accesToken", out var accesToken);

            var user = await _authService.getUserFromToken(accesToken);

            var result = await _taskService.DeleteTask(user.Id, id);

            if (!result)
            {
                return BadRequest("No se pudo encontrar la tarea a borrar.");
            }

            return Ok();
        }

        [HttpPut("updateTask")]
        [Authorize]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto dto)
        {
            HttpContext.Request.Cookies.TryGetValue("accesToken", out var accesToken);

            var user = await _authService.getUserFromToken(accesToken);
            var result = await _taskService.UpdateTask(dto, user.Id);

            if (!result) return BadRequest("No se encontro la tarea a actualizar.");

            return Ok("Se actualizo la tarea correctamente.");
        }

        [HttpGet("sortTaskByPriority")]
        [Authorize]
        public async Task<IActionResult> SortTasksByPriority()
        {
            HttpContext.Request.Cookies.TryGetValue("accesToken", out var accesToken);

            var user = await _authService.getUserFromToken(accesToken);
            var sortedTasks = await _taskService.SortTasksByPriority(user.Id);

            if (sortedTasks == null)
                return BadRequest("No se encontraron tareas.");

            return Ok(sortedTasks);
        }
    }
}
