using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;

namespace ToDoApi.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // Crear tarea
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        // Obtener todas las tareas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks()
        {
            return await _context.Tasks
                .Include(t => t.Comments)
                .ToListAsync();
        }

        // Obtener tarea por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskById(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return NotFound();

            return task;
        }

        // Actualizar tarea
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, TaskItem updatedTask)
        {
            if (id != updatedTask.Id) return BadRequest();

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null) return NotFound();

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.IsCompleted = updatedTask.IsCompleted;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Eliminar tarea
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
