namespace OnlineExamSystem.Models
{
    public class StudentExamAttempt
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime SubmitTime { get; set; }
        public double Score { get; set; }
        public bool IsSubmitted { get; set; }
        public Exam? Exam { get; set; }
    }
}