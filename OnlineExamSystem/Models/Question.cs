using OnlineExamSystem.Models;
using OnlineExamSystem.Models.Enums;

public class Question
{
    public int Id { get; set; }

    public string Content { get; set; } = "";

    public QuestionType Type { get; set; }

    public string OptionsJson { get; set; } = "";
    public string CorrectAnswerJson { get; set; } = "";

    public string? MatchingJson { get; set; }

    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }

    public int ExamId { get; set; }
    public decimal Score { get; set; }
    public int Order { get; set; }
    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}