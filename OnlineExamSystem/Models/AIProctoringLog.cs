using System.ComponentModel.DataAnnotations;

namespace OnlineExamSystem.Models
{
    public class AIProctoringLog
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public string EventType { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime TimeDetected { get; set; }
        public int WarningLevel { get; set; }
        public string? IPAddress { get; set; }
        public string? SnapshotPath { get; set; }
        public double ConfidenceScore { get; set; }
        public bool FromComputerVision { get; set; }
        public bool FromBrowserSecurity { get; set; }
        public bool FromSEB { get; set; }
        public bool IsConfirmedCheating { get; set; }
        public string? ConfirmedBy { get; set; }
        public DateTime? ConfirmedTime { get; set; }
    }
}