using OnlineExamSystem.Models;

namespace OnlineExamSystem.ViewModels
{
    public class ExamReportViewModel
    {
        public int ExamSessionId { get; set; }

        public string SessionName { get; set; } = "";

        public string ExamTitle { get; set; } = "";
        public int TotalStudents { get; set; }

        public int TotalAttempts { get; set; }

        public decimal AverageScore { get; set; }

        public decimal HighestScore { get; set; }

        public decimal LowestScore { get; set; }

        public int TotalViolations { get; set; }

        public int TotalAiViolations { get; set; }

        public int TotalCopyPasteViolations { get; set; }
        public int ExcellentCount { get; set; }

        public int GoodCount { get; set; }

        public int AverageCount { get; set; }

        public int WeakCount { get; set; }
        public List<ExamResult> Results { get; set; } = new();
    }
}