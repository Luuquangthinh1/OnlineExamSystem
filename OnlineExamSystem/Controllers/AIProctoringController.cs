using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Data;
using OnlineExamSystem.Models;

namespace OnlineExamSystem.Controllers
{
    public class AIProctoringController : Controller
    {
        private readonly OnlineExamSystemContext _context;

        public AIProctoringController(
            OnlineExamSystemContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult LogEvent(
            int examId,
            string eventType,
            string description,
            double confidenceScore = 0,
            string? snapshotPath = null)
        {
            var studentId =
                HttpContext.Session.GetInt32("UserId");

            if (studentId == null)
                return Unauthorized();

            int warningLevel = 0;

            bool fromComputerVision = false;
            bool fromBrowserSecurity = false;
            bool fromSEB = false;

            switch (eventType)
            {
                case "EXIT_FULLSCREEN":
                    warningLevel = 1;
                    fromBrowserSecurity = true;
                    break;

                case "SEB_CLOSED":
                    warningLevel = 1;
                    fromSEB = true;
                    break;

                case "PHONE":
                case "BOOK":
                case "MULTI_FACE":
                case "LOOK_AWAY":
                    fromComputerVision = true;
                    break;

                case "COPY":
                case "PASTE":
                case "SCREENSHOT":
                case "TAB_SWITCH":
                    fromBrowserSecurity = true;
                    break;
            }

            var log = new AIProctoringLog
            {
                StudentId = studentId.Value,

                ExamId = examId,

                EventType = eventType,

                Description = description,

                TimeDetected = DateTime.Now,

                WarningLevel = warningLevel,

                IPAddress =
                    HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString(),

                ConfidenceScore =
                    confidenceScore,

                SnapshotPath =
                    snapshotPath,

                FromComputerVision =
                    fromComputerVision,

                FromBrowserSecurity =
                    fromBrowserSecurity,

                FromSEB =
                    fromSEB,

                IsConfirmedCheating = false
            };

            _context.AIProctoringLogs.Add(log);

            _context.SaveChanges();

            int totalWarnings =
                _context.AIProctoringLogs
                .Where(x =>
                    x.StudentId == studentId.Value &&
                    x.ExamId == examId)
                .Sum(x => x.WarningLevel);

            bool autoSubmit =
                totalWarnings >= 5;

            return Json(new
            {
                success = true,

                warnings = totalWarnings,

                autoSubmit,

                eventType,

                confidenceScore
            });
        }

        [HttpGet]
        public IActionResult CheckSEB()
        {
            bool isSEB =
                Request.Headers.ContainsKey(
                    "X-SafeExamBrowser-ConfigKeyHash");

            return Json(new
            {
                isSEB
            });
        }

        [HttpPost]
        public IActionResult ConfirmCheating(int logId)
        {
            var log =
                _context.AIProctoringLogs
                .FirstOrDefault(x => x.Id == logId);

            if (log == null)
                return NotFound();

            log.IsConfirmedCheating = true;

            log.ConfirmedBy =
                User.Identity?.Name ??
                "Administrator";

            log.ConfirmedTime =
                DateTime.Now;

            _context.SaveChanges();

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public IActionResult CheckAllAnswered(
            int examId)
        {
            var studentId =
                HttpContext.Session
                .GetInt32("UserId");

            if (studentId == null)
                return Unauthorized();

            bool allAnswered =
                !_context.Questions
                .Where(q => q.ExamId == examId)
                .Any(q =>
                    !_context.StudentAnswers
                    .Any(a =>
                        a.QuestionId == q.Id &&
                        a.StudentId == studentId));

            return Json(new
            {
                allAnswered
            });
        }
    }
}