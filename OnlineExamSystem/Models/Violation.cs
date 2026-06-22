namespace OnlineExamSystem.Models
{
    public class Violation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Action { get; set; }
        public DateTime Time { get; set; }
    }
}
