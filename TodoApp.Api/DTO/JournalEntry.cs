namespace TodoApp.Api.DTO
{
    public class JournalEntry
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedUtc { get; set; }
        public string UserId { get; set; } = "";
    }
}
