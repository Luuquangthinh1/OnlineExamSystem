namespace OnlineExamSystem.Models
{
    public class AssignedExam
    {
        public int Id { get; set; }
        public string StudentUsername { get; set; } = string.Empty;
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
    }
}
