using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExamSystem.Migrations
{
    /// <inheritdoc />
    public partial class initOnlineExamSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIProctoringLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeDetected = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WarningLevel = table.Column<int>(type: "int", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SnapshotPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfidenceScore = table.Column<double>(type: "float", nullable: false),
                    FromComputerVision = table.Column<bool>(type: "bit", nullable: false),
                    FromBrowserSecurity = table.Column<bool>(type: "bit", nullable: false),
                    FromSEB = table.Column<bool>(type: "bit", nullable: false),
                    IsConfirmedCheating = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIProctoringLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamViolations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamViolations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OptionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswerJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Difficulty = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAIGenerated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AudioPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoginCount = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Violations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Violations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExamSessionId = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    AllowReview = table.Column<bool>(type: "bit", nullable: false),
                    ShowAnswerAfterSubmit = table.Column<bool>(type: "bit", nullable: false),
                    MaxAttempts = table.Column<int>(type: "int", nullable: false),
                    RandomQuestionOrder = table.Column<bool>(type: "bit", nullable: false),
                    RandomAnswerOrder = table.Column<bool>(type: "bit", nullable: false),
                    RandomQuestionCount = table.Column<int>(type: "int", nullable: false),
                    EnableFullscreen = table.Column<bool>(type: "bit", nullable: false),
                    FullscreenAlerts = table.Column<bool>(type: "bit", nullable: false),
                    DetectTabSwitch = table.Column<bool>(type: "bit", nullable: false),
                    DetectAltTab = table.Column<bool>(type: "bit", nullable: false),
                    DetectWindowBlur = table.Column<bool>(type: "bit", nullable: false),
                    DetectCopyPaste = table.Column<bool>(type: "bit", nullable: false),
                    DetectRightClick = table.Column<bool>(type: "bit", nullable: false),
                    DetectDevTools = table.Column<bool>(type: "bit", nullable: false),
                    DetectMultiMonitor = table.Column<bool>(type: "bit", nullable: false),
                    DetectIdle = table.Column<bool>(type: "bit", nullable: false),
                    EnableAIProctoring = table.Column<bool>(type: "bit", nullable: false),
                    EnableBehaviorAnalytics = table.Column<bool>(type: "bit", nullable: false),
                    EnableEyeTracking = table.Column<bool>(type: "bit", nullable: false),
                    EnableFaceTracking = table.Column<bool>(type: "bit", nullable: false),
                    EnableWebcamMonitoring = table.Column<bool>(type: "bit", nullable: false),
                    ShowCameraBeforeStart = table.Column<bool>(type: "bit", nullable: false),
                    SaveWebcamSnapshots = table.Column<bool>(type: "bit", nullable: false),
                    WebcamSnapshotInterval = table.Column<int>(type: "int", nullable: false),
                    DetectPhone = table.Column<bool>(type: "bit", nullable: false),
                    DetectOtherPerson = table.Column<bool>(type: "bit", nullable: false),
                    DetectBooksOrNotes = table.Column<bool>(type: "bit", nullable: false),
                    DetectMultipleFaces = table.Column<bool>(type: "bit", nullable: false),
                    DetectExternalDevices = table.Column<bool>(type: "bit", nullable: false),
                    MaxWarnings = table.Column<int>(type: "int", nullable: false),
                    EnableViolationPenalty = table.Column<bool>(type: "bit", nullable: false),
                    WarningPenaltyStart = table.Column<int>(type: "int", nullable: false),
                    PenaltyPerViolation = table.Column<double>(type: "float", nullable: false),
                    AutoSubmitWhenCheating = table.Column<bool>(type: "bit", nullable: false),
                    LockExamAfterCheating = table.Column<bool>(type: "bit", nullable: false),
                    ForceZeroScoreWhenCheating = table.Column<bool>(type: "bit", nullable: false),
                    BanRetakeAfterCheating = table.Column<bool>(type: "bit", nullable: false),
                    EnableAIEssayGrading = table.Column<bool>(type: "bit", nullable: false),
                    EnablePlagiarismDetection = table.Column<bool>(type: "bit", nullable: false),
                    EssayScore = table.Column<double>(type: "float", nullable: false),
                    EssayFeedback = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exams_ExamSessions_ExamSessionId",
                        column: x => x.ExamSessionId,
                        principalTable: "ExamSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignedExams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedExams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignedExams_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    QuestionBankId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_QuestionBanks_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalTable: "QuestionBanks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExamResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    ExamSessionId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ViolationCount = table.Column<int>(type: "int", nullable: false),
                    FullscreenViolationCount = table.Column<int>(type: "int", nullable: false),
                    AltTabCount = table.Column<int>(type: "int", nullable: false),
                    CopyPasteCount = table.Column<int>(type: "int", nullable: false),
                    BlurCount = table.Column<int>(type: "int", nullable: false),
                    DevToolsCount = table.Column<int>(type: "int", nullable: false),
                    SuspiciousAiDetected = table.Column<bool>(type: "bit", nullable: false),
                    CopyPasteDetected = table.Column<bool>(type: "bit", nullable: false),
                    FullscreenViolation = table.Column<bool>(type: "bit", nullable: false),
                    AltTabDetected = table.Column<bool>(type: "bit", nullable: false),
                    WindowBlurDetected = table.Column<bool>(type: "bit", nullable: false),
                    DevToolsDetected = table.Column<bool>(type: "bit", nullable: false),
                    RightClickDetected = table.Column<bool>(type: "bit", nullable: false),
                    MultiMonitorDetected = table.Column<bool>(type: "bit", nullable: false),
                    IdleDetected = table.Column<bool>(type: "bit", nullable: false),
                    WebcamDisabled = table.Column<bool>(type: "bit", nullable: false),
                    PhoneDetected = table.Column<bool>(type: "bit", nullable: false),
                    BookDetected = table.Column<bool>(type: "bit", nullable: false),
                    OtherPersonDetected = table.Column<bool>(type: "bit", nullable: false),
                    MultipleFaceDetected = table.Column<bool>(type: "bit", nullable: false),
                    AutoSubmitted = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    ForceZeroScore = table.Column<bool>(type: "bit", nullable: false),
                    BannedFromRetake = table.Column<bool>(type: "bit", nullable: false),
                    LockReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViolationDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ViolationAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AiSuspiciousScore = table.Column<double>(type: "float", nullable: true),
                    EyeTrackingScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FaceTrackingScore = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    WebcamSnapshotPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViolationImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PenaltyScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EssayScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TeacherComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEssayGraded = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResults_ExamSessions_ExamSessionId",
                        column: x => x.ExamSessionId,
                        principalTable: "ExamSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExamResults_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamResults_User_StudentId",
                        column: x => x.StudentId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OptionsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswerJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatchingJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamSessionId = table.Column<int>(type: "int", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    WrongAnswers = table.Column<int>(type: "int", nullable: false),
                    UnansweredQuestions = table.Column<int>(type: "int", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ViolationCount = table.Column<int>(type: "int", nullable: false),
                    TabSwitchCount = table.Column<int>(type: "int", nullable: false),
                    SuspiciousAiDetected = table.Column<bool>(type: "bit", nullable: false),
                    CopyPasteDetected = table.Column<bool>(type: "bit", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Results_ExamSessions_ExamSessionId",
                        column: x => x.ExamSessionId,
                        principalTable: "ExamSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Results_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentExamAttempt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ExamId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmitTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    IsSubmitted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentExamAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentExamAttempt_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnsweredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamResultId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_ExamResults_ExamResultId",
                        column: x => x.ExamResultId,
                        principalTable: "ExamResults",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedExams_ExamId",
                table: "AssignedExams",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_ExamId",
                table: "ExamQuestions",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_QuestionBankId",
                table: "ExamQuestions",
                column: "QuestionBankId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_ExamId",
                table: "ExamResults",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_ExamSessionId",
                table: "ExamResults",
                column: "ExamSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_StudentId",
                table: "ExamResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ExamSessionId",
                table: "Exams",
                column: "ExamSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExamId",
                table: "Questions",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_ExamId",
                table: "Results",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_ExamSessionId",
                table: "Results",
                column: "ExamSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_UserId",
                table: "Results",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_ExamResultId",
                table: "StudentAnswers",
                column: "ExamResultId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExamAttempt_ExamId",
                table: "StudentExamAttempt",
                column: "ExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIProctoringLogs");

            migrationBuilder.DropTable(
                name: "AssignedExams");

            migrationBuilder.DropTable(
                name: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "ExamViolations");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "StudentExamAttempt");

            migrationBuilder.DropTable(
                name: "Violations");

            migrationBuilder.DropTable(
                name: "QuestionBanks");

            migrationBuilder.DropTable(
                name: "ExamResults");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "ExamSessions");
        }
    }
}
