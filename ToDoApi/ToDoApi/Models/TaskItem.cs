using System.ComponentModel.DataAnnotations;

namespace ToDoApi.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
