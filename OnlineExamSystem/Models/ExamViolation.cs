namespace OnlineExamSystem.Models
{
    public class ExamViolation
    {
        public int Id { get; set; }

        public int ExamId { get; set; }

        public int StudentId { get; set; }

        public string Type { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
