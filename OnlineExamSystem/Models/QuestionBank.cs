using OnlineExamSystem.Models.Enums;

namespace OnlineExamSystem.Models
{
    public class QuestionBank
    {
        public int Id { get; set; }

        public string Content { get; set; } = "";

        public QuestionType Type { get; set; }

        public string OptionsJson { get; set; } = "[]";

        public string CorrectAnswerJson { get; set; } = "[]";

        public string? Subject { get; set; }

        public string? Difficulty { get; set; }

        public bool IsAIGenerated { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;
        public decimal Score { get; set; }
        public string? AudioPath { get; set; }
    }
}