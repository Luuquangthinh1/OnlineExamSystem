namespace OnlineExamSystem.Models
{
    public class ExamQuestion
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public Exam? Exam { get; set; }
        public QuestionBank? QuestionBank { get; set; }
    }
}
