using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExamSystem.Models
{
    public class Result
    {
        [Key]
        public int Id { get; set; }

        public int ExamSessionId { get; set; }

        public int ExamId { get; set; }

        public int UserId { get; set; }

        public decimal Score { get; set; }

        public int CorrectAnswers { get; set; }

        public int WrongAnswers { get; set; }

        public int UnansweredQuestions { get; set; }

        public int Duration { get; set; }

        public int ViolationCount { get; set; }

        public int TabSwitchCount { get; set; }

        public bool SuspiciousAiDetected { get; set; }

        public bool CopyPasteDetected { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime SubmittedAt { get; set; }

        [ForeignKey(nameof(ExamSessionId))]
        public virtual ExamSession? ExamSession { get; set; }

        [ForeignKey(nameof(ExamId))]
        public virtual Exam? Exam { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
    }
}