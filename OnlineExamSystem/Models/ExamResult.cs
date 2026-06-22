namespace OnlineExamSystem.Models
{
    public class ExamResult
    {
        public int Id { get; set; }
        public int ExamId { get; set; }

        public Exam? Exam { get; set; }

        public int ExamSessionId { get; set; }

        public ExamSession? ExamSession
        { get; set; }

        public int StudentId { get; set; }

        public User? Student { get; set; }

        public decimal Score { get; set; }

        public DateTime SubmittedAt
        { get; set; }
        public int ViolationCount
        { get; set; } = 0;
        public int FullscreenViolationCount
        { get; set; } = 0;
        public int AltTabCount
        { get; set; } = 0;
        public int CopyPasteCount
        { get; set; } = 0;
        public int BlurCount
        { get; set; } = 0;
        public int DevToolsCount
        { get; set; } = 0;
        public bool SuspiciousAiDetected
        { get; set; } = false;
        public bool CopyPasteDetected
        { get; set; } = false;
        public bool FullscreenViolation
        { get; set; } = false;
        public bool AltTabDetected
        { get; set; } = false;
        public bool WindowBlurDetected
        { get; set; } = false;
        public bool DevToolsDetected
        { get; set; } = false;
        public bool RightClickDetected
        { get; set; } = false;
        public bool MultiMonitorDetected
        { get; set; } = false;
        public bool IdleDetected
        { get; set; } = false;
        public bool WebcamDisabled
        { get; set; } = false;
        public bool PhoneDetected
        { get; set; } = false;
        public bool BookDetected
        { get; set; } = false;
        public bool OtherPersonDetected
        { get; set; } = false;
        public bool MultipleFaceDetected
        { get; set; } = false;
        public bool AutoSubmitted
        { get; set; } = false;
        public bool IsLocked
        { get; set; } = false;
        public bool ForceZeroScore
        { get; set; } = false;
        public bool BannedFromRetake
        { get; set; } = false;
        public string? LockReason
        { get; set; }

        public string? ViolationDetails
        { get; set; }
        public DateTime? LockedAt
        { get; set; }

        public DateTime? ViolationAt
        { get; set; }
        public double? AiSuspiciousScore
        { get; set; }

        public decimal? EyeTrackingScore
        { get; set; }

        public decimal? FaceTrackingScore
        { get; set; }
        public string? WebcamSnapshotPath
        { get; set; }

        public string? ViolationImagePath
        { get; set; }
        public decimal PenaltyScore
        {
            get;
            set;
        } = 0;

        public decimal FinalScore
        {
            get;
            set;
        } = 0;
        public decimal EssayScore
        {
            get; set;
        }

        public string? TeacherComment { get; set; }
        public bool IsEssayGraded { get; set; }
        public ICollection<StudentAnswer>
    StudentAnswers
        {
            get; set;
        }
    = new List<StudentAnswer>();
    }
}