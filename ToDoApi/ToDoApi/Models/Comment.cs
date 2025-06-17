namespace ToDoApi.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public bool IsUpdated { get; set; } = false;

        public int TaskItemId { get; set; }
        public TaskItem TaskItem {get; set;} =null!;

        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    }
}
