namespace OnlineExamSystem.Models
{
    public class ExamSession
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public List<Exam> Exams { get; set; } = new();
        public ICollection<ExamResult> ExamResults
        { get; set; }
        = new List<ExamResult>();
    }
}
