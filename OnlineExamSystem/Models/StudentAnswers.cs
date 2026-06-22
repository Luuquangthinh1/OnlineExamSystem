using System.ComponentModel.DataAnnotations;

namespace OnlineExamSystem.Models
{
    public class StudentAnswer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Required]
        public int QuestionId { get; set; }
        public string? Answer { get; set; }
        public DateTime AnsweredAt { get; set; } = DateTime.Now;
        public string? AnswerText { get; set; }
    }
}
