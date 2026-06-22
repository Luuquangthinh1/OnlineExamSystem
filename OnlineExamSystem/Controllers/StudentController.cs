using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Data;
using OnlineExamSystem.Models;
using OnlineExamSystem.Models.Enums;
using System.Text.Json;
public class StudentController : Controller
{
    private readonly OnlineExamSystemContext _context;
    public StudentController(OnlineExamSystemContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var sessions = _context.ExamSessions

            .Include(x =>
                x.Exams
                    .Where(e => e.IsPublished))

            .ThenInclude(e =>
                e.Questions)

            .ToList();

        return View(sessions);
    }
    public IActionResult SessionExams(int id)
    {
        var username =
            HttpContext.Session
            .GetString("User");

        var session =
            _context.ExamSessions

            .Include(x =>
                x.Exams.Where(e =>
                    e.IsPublished))

            .ThenInclude(e =>
                e.Questions)

            .Include(x =>
                x.Exams)

            .ThenInclude(e =>
                e.ExamResults)

            .FirstOrDefault(x =>
                x.Id == id);

        if (session == null)
        {
            return NotFound();
        }

        return View(session);
    }
    public async Task<IActionResult> Start(int id)
    {
        var exam = _context.Exams
            .Include(x => x.Questions)
            .FirstOrDefault(x =>
                x.Id == id
                && x.IsPublished);

        if (exam == null)
        {
            return NotFound();
        }

        var session = await _context.ExamSessions
            .FirstOrDefaultAsync(x =>
                x.Id == exam.ExamSessionId);

        var now = DateTime.Now;
        if (session != null)
        {
            if (now < session.StartTime)
            {
                TempData["Error"] =
                    "Ca thi chưa bắt đầu!";

                return RedirectToAction(
                    "Index");
            }

            if (now > session.EndTime)
            {
                TempData["Error"] =
                    "Ca thi đã kết thúc!";

                return RedirectToAction(
                    "Index");
            }
        }
        if (now < exam.StartTime)
        {
            TempData["Error"] =
                "Đề thi chưa mở!";

            return RedirectToAction(
                "Index");
        }

        if (now > exam.EndTime)
        {
            TempData["Error"] =
                "Đề thi đã đóng!";

            return RedirectToAction(
                "Index");
        }
        var userId =
            HttpContext.Session
            .GetInt32("UserId");

        if (userId == null)
        {
            TempData["Error"] =
        "Bạn cần đăng nhập để vào thi!";
            return RedirectToAction(
                "Login",
                "Account");
        }
        var role = HttpContext.Session.GetString("Role");
        if (role != "Student")
        {
            TempData["Error"] =
                "Chỉ học sinh mới được vào thi!";

            return RedirectToAction(
                "Login",
                "Account");
        }

        var attemptCount =
            _context.ExamResults
            .Count(x =>
                x.ExamId == id
                && x.StudentId == userId);

        if (attemptCount >= exam.MaxAttempts)
        {
            var locked =
    _context.ExamResults.Any(x =>
        x.ExamId == id
        &&
        x.StudentId == userId
        &&
        x.Score == 0);

            if (locked &&
                exam.LockExamAfterCheating)
            {
                TempData["Error"] =
                    "Bạn đã bị khóa thi do gian lận!";

                return RedirectToAction(
                    "Index");
            }
            TempData["Error"] =
                "Bạn đã vượt quá số lần làm theo quy định.";

            return RedirectToAction(
                "Index");
        }

        return View(exam);
    }
    [HttpPost]
    public IActionResult AutoSave(int examId, Dictionary<string, string[]> answers)
    {
        HttpContext.Session.SetString("autosave_" + examId, JsonSerializer.Serialize(answers));
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> SubmitExam(int examId)
    {
        var exam = _context.Exams
            .Include(x => x.Questions)
            .FirstOrDefault(x => x.Id == examId);

        if (exam == null)
            return NotFound();

        var userId =
            HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return RedirectToAction(
                "Login",
                "Account");
        }

        var attemptCount =
            _context.ExamResults
            .Count(x =>
                x.ExamId == examId &&
                x.StudentId == userId);

        if (attemptCount >= exam.MaxAttempts)
        {
            TempData["Error"] =
                "Bạn đã vượt quá số lần làm theo quy định.";

            return RedirectToAction("Index");
        }

        if (DateTime.Now < exam.StartTime)
        {
            TempData["Error"] =
                "Đề thi chưa mở!";

            return RedirectToAction("Index");
        }

        if (DateTime.Now > exam.EndTime)
        {
            TempData["Error"] =
                "Đã hết thời gian làm bài!";

            return RedirectToAction("Index");
        }

        decimal score = 0m;

        bool cheatingDetected = false;

        string cheatingReason = string.Empty;
        int warnings =
    HttpContext.Session
    .GetInt32(
        "warnings_" + examId)
    ?? 0;

        if (warnings >= exam.MaxWarnings)
        {
            cheatingDetected = true;

            cheatingReason =
                "Vượt quá số lần cảnh báo";
        }
        if (Request.Form["phoneDetected"] == "true")
        {
            cheatingDetected = true;

            cheatingReason =
                "Phát hiện điện thoại";
        }

        if (Request.Form["bookDetected"] == "true")
        {
            cheatingDetected = true;

            cheatingReason =
                "Phát hiện tài liệu";
        }

        if (Request.Form["otherPersonDetected"] == "true")
        {
            cheatingDetected = true;

            cheatingReason =
                "Phát hiện người khác";
        }
        decimal essayTotalScore = 0m;

        string essayFeedback = "";
        foreach (var q in exam.Questions)
        {
            var key = $"q_{q.Id}";

            var userAnswers =
                Request.Form[key].ToList();

            var correctAnswers =
                JsonSerializer.Deserialize<List<string>>
                (
                    q.CorrectAnswerJson ?? "[]"
                ) ?? new();
            if (q.Type == QuestionType.SingleChoice)
            {
                if (userAnswers.FirstOrDefault()?.Trim().ToLower()
                    ==
                    correctAnswers.FirstOrDefault()?.Trim().ToLower())
                {
                    score += q.Score;
                }
            }
            else if (q.Type == QuestionType.TrueFalse)
            {
                if (userAnswers.FirstOrDefault()?.Trim().ToLower()
                    ==
                    correctAnswers.FirstOrDefault()?.Trim().ToLower())
                {
                    score += q.Score;
                }
            }
            else if (q.Type == QuestionType.MultipleChoice)
            {
                if (userAnswers
                    .Select(x => x?.Trim().ToLower() ?? "")
                    .OrderBy(x => x)
                    .SequenceEqual(
                        correctAnswers
                        .Select(x => x?.Trim().ToLower() ?? "")
                        .OrderBy(x => x)))
                {
                    score += q.Score;
                }
            }
            else if (q.Type == QuestionType.FillBlank)
            {
                if (userAnswers.FirstOrDefault()?.Trim().ToLower()
                    ==
                    correctAnswers.FirstOrDefault()?.Trim().ToLower())
                {
                    score += q.Score;
                }
            }
            else if (q.Type == QuestionType.Essay)
            {
                string studentAnswer =
                    userAnswers.FirstOrDefault()
                    ?? "";

                essayFeedback +=
            $@"
================================

CÂU HỎI:
{q.Content}

--------------------------------

BÀI LÀM SINH VIÊN:
{studentAnswer}

================================
";

            }
            else if (q.Type == QuestionType.Matching)
            {
                bool correct = true;

                for (int i = 0; i < correctAnswers.Count; i++)
                {
                    var userAnswer =
                        Request.Form[$"q_{q.Id}_{i}"]
                        .ToString()
                        .Trim()
                        .ToLower();

                    var rightAnswer =
                        correctAnswers[i]
                        .Trim()
                        .ToLower();

                    if (userAnswer != rightAnswer)
                    {
                        correct = false;
                        break;
                    }
                }

                if (correct)
                {
                    score += q.Score;
                }
            }
        }
        if (cheatingDetected)
        {
            score = 0;

            AddViolation(
                examId,
                userId.Value,
                cheatingReason);

            TempData["Error"] =
                cheatingReason
                + " - Bài thi bị 0 điểm!";
        }
        _context.ExamResults.Add(new ExamResult
        {
            ExamId = examId,
            ExamSessionId = exam.ExamSessionId,
            StudentId = userId.Value,
            Score = score,
            EssayScore = 0m,
            SubmittedAt = DateTime.Now
        });

        _context.SaveChanges();
        if (cheatingDetected &&
    exam.LockExamAfterCheating)
        {
            HttpContext.Session.SetString(
                "locked_" + examId,
                "true");
        }

        decimal totalScore =
    exam.Questions.Sum(x => (decimal)x.Score);

        decimal objectiveScore =
    score - essayTotalScore;

        ViewBag.TotalScore =
            totalScore;

        ViewBag.ObjectiveScore =
            objectiveScore;

        ViewBag.EssayScore =
            essayTotalScore;
        TempData["Success"] =
            "Nộp bài thành công!";

        return View("Result");
    }
    private void AddViolation(
    int examId,
    int studentId,
    string type)
    {
        _context.ExamViolations.Add(
            new ExamViolation
            {
                ExamId = examId,
                StudentId = studentId,
                Type = type,
                CreatedAt = DateTime.Now
            });

        _context.SaveChanges();
    }

}