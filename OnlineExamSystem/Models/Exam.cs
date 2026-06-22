using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineExamSystem.Models
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = "";

        public string? Description { get; set; }

        [Required]
        public int Duration { get; set; }

        public double TotalScore
        {
            get;
            set;
        } = 10;

        public DateTime CreatedAt
        {
            get;
            set;
        } = DateTime.Now;

        public DateTime StartTime
        {
            get;
            set;
        }

        public DateTime EndTime
        {
            get;
            set;
        }
        [Required]
        public int ExamSessionId
        {
            get;
            set;
        }

        [ForeignKey(nameof(ExamSessionId))]
        public virtual ExamSession?
        ExamSession
        {
            get;
            set;
        }
        public bool IsPublished
        {
            get;
            set;
        } = false;

        public bool AllowReview
        {
            get;
            set;
        } = true;

        public bool ShowAnswerAfterSubmit
        {
            get;
            set;
        } = false;

        public int MaxAttempts
        {
            get;
            set;
        } = 1;
        public bool RandomQuestionOrder
        {
            get;
            set;
        } = false;

        public bool RandomAnswerOrder
        {
            get;
            set;
        } = false;

        public int RandomQuestionCount
        {
            get;
            set;
        } = 0;
        public bool EnableFullscreen
        {
            get;
            set;
        } = true;

        public bool FullscreenAlerts
        {
            get;
            set;
        } = true;
        public bool DetectTabSwitch
        {
            get;
            set;
        } = true;

        public bool DetectAltTab
        {
            get;
            set;
        } = true;

        public bool DetectWindowBlur
        {
            get;
            set;
        } = true;

        public bool DetectCopyPaste
        {
            get;
            set;
        } = true;

        public bool DetectRightClick
        {
            get;
            set;
        } = true;

        public bool DetectDevTools
        {
            get;
            set;
        } = true;

        public bool DetectMultiMonitor
        {
            get;
            set;
        } = true;

        public bool DetectIdle
        {
            get;
            set;
        } = true;
        public bool EnableAIProctoring
        {
            get;
            set;
        } = true;

        public bool EnableBehaviorAnalytics
        {
            get;
            set;
        } = true;

        public bool EnableEyeTracking
        {
            get;
            set;
        } = true;

        public bool EnableFaceTracking
        {
            get;
            set;
        } = true;
        public bool EnableWebcamMonitoring
        {
            get;
            set;
        } = true;

        public bool ShowCameraBeforeStart
        {
            get;
            set;
        } = true;

        public bool SaveWebcamSnapshots
        {
            get;
            set;
        } = true;

        public int WebcamSnapshotInterval
        {
            get;
            set;
        } = 30;
        public bool DetectPhone
        {
            get;
            set;
        } = true;

        public bool DetectOtherPerson
        {
            get;
            set;
        } = true;

        public bool DetectBooksOrNotes
        {
            get;
            set;
        } = true;

        public bool DetectMultipleFaces
        {
            get;
            set;
        } = true;

        public bool DetectExternalDevices
        {
            get;
            set;
        } = false;
        public int MaxWarnings
        {
            get;
            set;
        } = 5;
        public bool EnableViolationPenalty
        {
            get;
            set;
        } = true;
        public int WarningPenaltyStart
        {
            get;
            set;
        } = 2;
        public double PenaltyPerViolation
        {
            get;
            set;
        } = 0.5;

        public bool AutoSubmitWhenCheating
        {
            get;
            set;
        } = true;

        public bool LockExamAfterCheating
        {
            get;
            set;
        } = true;

        public bool ForceZeroScoreWhenCheating
        {
            get;
            set;
        } = true;

        public bool BanRetakeAfterCheating
        {
            get;
            set;
        } = true;
        public bool EnableAIEssayGrading
        {
            get;
            set;
        } = true;

        public bool EnablePlagiarismDetection
        {
            get;
            set;
        } = true;
        public virtual ICollection<Question>
        Questions
        {
            get;
            set;
        }
        = new List<Question>();

        public virtual List<StudentExamAttempt>
        StudentExamAttempts
        {
            get;
            set;
        } = new();
        [NotMapped]
        public List<StudentAnswer>
        StudentAnswers
        {
            get
            {
                return Questions?
                    .SelectMany(q =>
                        q.StudentAnswers)
                    .ToList()

                    ?? new List<StudentAnswer>();
            }
        }
        public ICollection<ExamResult>? ExamResults { get; set; }
        public double EssayScore
        {
            get; set;
        }

        public string EssayFeedback
        {
            get; set;
        } = "";
    }
}