using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Models;

namespace OnlineExamSystem.Data
{
    public class OnlineExamSystemContext : DbContext
    {
        public OnlineExamSystemContext(DbContextOptions<OnlineExamSystemContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Violation> Violations { get; set; }

        public DbSet<OnlineExamSystem.Models.User> User { get; set; } = default!;

        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<AIProctoringLog> AIProctoringLogs { get; set; }
        public DbSet<AssignedExam> AssignedExams { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<QuestionBank> QuestionBanks { get; set; }
        public DbSet<ExamViolation> ExamViolations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExamResult>()
    .HasOne(r => r.ExamSession)
    .WithMany(e => e.ExamResults)
    .HasForeignKey(r => r.ExamSessionId)
    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Exam)
                .WithMany()
                .HasForeignKey(r => r.ExamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
