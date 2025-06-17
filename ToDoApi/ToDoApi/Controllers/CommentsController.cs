using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;

namespace ToDoApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks/{taskId}/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        // Crear comentario
        [HttpPost]
        public async Task<ActionResult<Comment>> CreateComment(int taskId, [FromBody] Comment comment)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return NotFound("Tarea no encontrada");

            comment.TaskItemId = taskId;
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComments), new { taskId }, comment);
        }

        // Crear respuesta a un comentario
        [HttpPost("{parentCommentId}/reply")]
        public async Task<ActionResult<Comment>> ReplyToComment(int taskId, int parentCommentId, [FromBody] Comment reply)
        {
            var parent = await _context.Comments.FindAsync(parentCommentId);
            if (parent == null || parent.TaskItemId != taskId) return NotFound("Comentario padre no encontrado");

            reply.TaskItemId = taskId;
            reply.ParentCommentId = parentCommentId;
            _context.Comments.Add(reply);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComments), new { taskId }, reply);
        }

        // Obtener comentarios de una tarea
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int taskId)
        {
            var taskExists = await _context.Tasks.AnyAsync(t => t.Id == taskId);
            if (!taskExists) return NotFound("Tarea no encontrada");

            var comments = await _context.Comments
                .Where(c => c.TaskItemId == taskId && c.ParentCommentId == null)
                .Include(c => c.Replies)
                .ToListAsync();

            return comments;
        }

        // Actualizar comentario
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(int taskId, int commentId, [FromBody] Comment updatedComment)
        {
            if (commentId != updatedComment.Id) return BadRequest();

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.TaskItemId == taskId);
            if (comment == null) return NotFound();

            comment.Content = updatedComment.Content;
            comment.IsUpdated = true;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Eliminar comentario
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int taskId, int commentId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId && c.TaskItemId == taskId);
            if (comment == null) return NotFound();

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
