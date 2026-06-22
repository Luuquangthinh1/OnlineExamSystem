using DocumentFormat.OpenXml.Packaging;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Data;
using OnlineExamSystem.Models;
using OnlineExamSystem.Models.Enums;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
public class TeacherController : Controller
{
    private readonly OnlineExamSystemContext _context;
    public TeacherController(OnlineExamSystemContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var examSessions = _context.ExamSessions
            .Include(x => x.Exams)
            .OrderByDescending(x => x.StartTime)
            .ToList();
        return View(examSessions);
    }
    public IActionResult CreateSession() => View();
    [HttpPost]
    public IActionResult CreateSession(ExamSession model)
    {
        if (model == null)
        {
            TempData["Error"] = "Không tìm thấy dữ liệu!";
            return View();
        }
        if (model.EndTime <= model.StartTime)
        {
            TempData["Error"] = "Thời gian không hợp lệ!";
            return View(model);
        }
        _context.ExamSessions.Add(model);
        _context.SaveChanges();
        TempData["Success"] =
            $"Đã tạo kỳ thi: {model.Name} | " +
            $"Bắt đầu: {model.StartTime:dd/MM/yyyy HH:mm} | " +
            $"Kết thúc: {model.EndTime:dd/MM/yyyy HH:mm}";
        return RedirectToAction("Index");
    }

    public IActionResult EditSession(int id)
    {
        var s = _context.ExamSessions.Find(id);
        return s == null ? NotFound() : View(s);
    }

    [HttpPost]
    public IActionResult EditSession(ExamSession model)
    {
        var s = _context.ExamSessions.Find(model.Id);
        if (s == null) return NotFound();

        if (s.EndTime < DateTime.Now)
        {
            TempData["Error"] = "Kỳ thi đã kết thúc!";
            return RedirectToAction("Index");
        }

        s.Name = model.Name;
        s.StartTime = model.StartTime;
        s.EndTime = model.EndTime;

        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    public IActionResult DeleteSession(int id)
    {
        var s = _context.ExamSessions.Find(id);
        if (s != null)
        {
            _context.ExamSessions.Remove(s);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
    public IActionResult CreateExam(int examSessionId)
    {
        ViewBag.QuestionBanks =
        _context.QuestionBanks
        .OrderByDescending(x => x.Id)
        .ToList();
        var session = _context.ExamSessions
            .FirstOrDefault(x => x.Id == examSessionId);
        if (session == null)
            return NotFound();
        if (session.EndTime < DateTime.Now)
        {
            TempData["Error"] =
                "Kỳ thi đã kết thúc!";

            return RedirectToAction(
                "ExamSessionDetails",
                new { id = examSessionId });
        }
        var model = new Exam
        {
            ExamSessionId = examSessionId
        };
        model.ExamSessionId = examSessionId;
        model.Duration = 0;
        model.MaxAttempts = 0;
        model.MaxWarnings = 0;
        model.IsPublished = false;
        model.ShowAnswerAfterSubmit = false;
        model.EnableFullscreen = false;
        model.DetectTabSwitch = false;
        model.DetectCopyPaste = false;
        model.DetectRightClick = false;
        model.DetectWindowBlur = false;
        model.DetectDevTools = false;
        model.DetectMultiMonitor = false;
        model.EnableWebcamMonitoring = false;
        model.DetectIdle = false;
        model.DetectAltTab = false;
        model.AutoSubmitWhenCheating = false;
        model.LockExamAfterCheating = false;
        model.EnableBehaviorAnalytics = false;
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult>
 CreateExam(
     Exam model,
     IFormFile? file,
     List<int>? selectedQuestions,
     double EssayScore = 1)
    {
        var session =
            await _context.ExamSessions
            .FirstOrDefaultAsync(x =>
                x.Id ==
                model.ExamSessionId);

        if (session == null)
            return NotFound();

        if (session.EndTime < DateTime.Now)
        {
            TempData["Error"] =
                "Kỳ thi đã kết thúc!";

            return RedirectToAction(
                "ExamSessionDetails",
                new
                {
                    id = model.ExamSessionId
                });
        }

        if (model.Duration <= 0)
            model.Duration = 60;

        if (model.MaxAttempts <= 0)
            model.MaxAttempts = 1;

        if (model.MaxWarnings < 0)
            model.MaxWarnings = 5;

        if (!ModelState.IsValid)
        {
            ViewBag.QuestionBank =
                await _context.QuestionBanks
                .ToListAsync();

            return View(model);
        }

        if (model.EndTime <= model.StartTime)
        {
            TempData["Error"] =
                "Thời gian kết thúc phải lớn hơn thời gian bắt đầu!";

            ViewBag.QuestionBank =
                await _context.QuestionBanks
                .ToListAsync();

            return View(model);
        }

        if (model.StartTime < session.StartTime)
        {
            TempData["Error"] =
                "Thời gian mở đề không được nhỏ hơn thời gian mở ca thi!";

            ViewBag.QuestionBank =
                await _context.QuestionBanks
                .ToListAsync();

            return View(model);
        }

        if (model.EndTime > session.EndTime)
        {
            TempData["Error"] =
                "Thời gian đóng đề không được vượt quá thời gian kết thúc ca thi!";

            ViewBag.QuestionBank =
                await _context.QuestionBanks
                .ToListAsync();

            return View(model);
        }

        bool hasWarningSetting =
    model.EnableFullscreen
    || model.DetectTabSwitch
    || model.DetectAltTab
    || model.DetectWindowBlur
    || model.DetectCopyPaste
    || model.DetectRightClick
    || model.DetectDevTools
    || model.DetectMultiMonitor
    || model.DetectIdle
    || model.EnableAIProctoring
    || model.EnableBehaviorAnalytics
    || model.EnableEyeTracking
    || model.EnableFaceTracking
    || model.EnableWebcamMonitoring
    || model.ShowCameraBeforeStart
    || model.SaveWebcamSnapshots
    || model.DetectPhone
    || model.DetectOtherPerson
    || model.DetectBooksOrNotes
    || model.DetectMultipleFaces
    || model.DetectExternalDevices
    || model.AutoSubmitWhenCheating
    || model.LockExamAfterCheating
    || model.ForceZeroScoreWhenCheating
    || model.BanRetakeAfterCheating
    || model.EnableViolationPenalty
    || model.EnableAIEssayGrading
    || model.EnablePlagiarismDetection;

        if (!hasWarningSetting)
        {
            TempData["Error"] =
                "Bạn phải chọn ít nhất 1 chế độ giám sát, chống gian lận hoặc cảnh báo trước khi tạo đề thi!";

            ViewBag.QuestionBank =
                await _context.QuestionBanks
                .ToListAsync();

            return View(model);
        }

        var exam = new Exam
        {
            Title = model.Title,

            Description = model.Description,

            Duration = model.Duration,

            TotalScore = 10,

            StartTime = model.StartTime,

            EndTime = model.EndTime,

            ExamSessionId =
         model.ExamSessionId,

            IsPublished = true,
            AllowReview =
         model.AllowReview,

            ShowAnswerAfterSubmit =
         model.ShowAnswerAfterSubmit,

            MaxAttempts =
         model.MaxAttempts,
            RandomQuestionOrder =
         model.RandomQuestionOrder,

            RandomAnswerOrder =
         model.RandomAnswerOrder,

            RandomQuestionCount =
         model.RandomQuestionCount,

            EnableFullscreen =
         model.EnableFullscreen,

            FullscreenAlerts =
         model.FullscreenAlerts,
            DetectTabSwitch =
         model.DetectTabSwitch,

            DetectAltTab =
         model.DetectAltTab,

            DetectWindowBlur =
         model.DetectWindowBlur,

            DetectCopyPaste =
         model.DetectCopyPaste,

            DetectRightClick =
         model.DetectRightClick,

            DetectDevTools =
         model.DetectDevTools,

            DetectMultiMonitor =
         model.DetectMultiMonitor,

            DetectIdle =
         model.DetectIdle,
            EnableAIProctoring =
         model.EnableAIProctoring,

            EnableBehaviorAnalytics =
         model.EnableBehaviorAnalytics,

            EnableEyeTracking =
         model.EnableEyeTracking,

            EnableFaceTracking =
         model.EnableFaceTracking,
            EnableWebcamMonitoring =
         model.EnableWebcamMonitoring,

            ShowCameraBeforeStart =
         model.ShowCameraBeforeStart,

            SaveWebcamSnapshots =
         model.SaveWebcamSnapshots,

            WebcamSnapshotInterval =
         model.WebcamSnapshotInterval,
            DetectPhone =
         model.DetectPhone,

            DetectOtherPerson =
         model.DetectOtherPerson,

            DetectBooksOrNotes =
         model.DetectBooksOrNotes,

            DetectMultipleFaces =
         model.DetectMultipleFaces,

            DetectExternalDevices =
         model.DetectExternalDevices,
            MaxWarnings =
         model.MaxWarnings,

            AutoSubmitWhenCheating =
         model.AutoSubmitWhenCheating,

            LockExamAfterCheating =
         model.LockExamAfterCheating,

            ForceZeroScoreWhenCheating =
         model.ForceZeroScoreWhenCheating,

            BanRetakeAfterCheating =
         model.BanRetakeAfterCheating,
            EnableViolationPenalty =
         model.EnableViolationPenalty,

            WarningPenaltyStart =
         model.WarningPenaltyStart,

            PenaltyPerViolation =
         model.PenaltyPerViolation,

            EnableAIEssayGrading =
         model.EnableAIEssayGrading,

            EnablePlagiarismDetection =
         model.EnablePlagiarismDetection
        };

        _context.Exams.Add(exam);

        await _context.SaveChangesAsync();
        if (file != null)
        {
            await UploadQuestionsInternal(
                exam.Id,
                file);
        }

        if (selectedQuestions != null
            &&
            selectedQuestions.Any())
        {
            var bankQuestions =
                await _context.QuestionBanks
                .Where(x =>
                    selectedQuestions
                    .Contains(x.Id))
                .ToListAsync();

            foreach (var qb in bankQuestions)
            {
                var q = new Question
                {
                    ExamId = exam.Id,

                    Content =
                qb.Content ?? "",

                    Type = qb.Type,

                    Score = 0,

                    OptionsJson =
                qb.OptionsJson ?? "[]",

                    CorrectAnswerJson =
                qb.CorrectAnswerJson ?? "[]"
                };

                _context.Questions.Add(q);
            }

            await _context.SaveChangesAsync();
        }

        var questions =
            await _context.Questions
            .Where(x =>
                x.ExamId == exam.Id)
            .ToListAsync();

        if (!questions.Any())
        {
            TempData["Error"] =
                "Đề thi chưa có câu hỏi!";

            return RedirectToAction(
                "EditExam",
                new
                {
                    id = exam.Id
                });
        }

        foreach (var q in questions)
        {
            q.Content ??= "";

            q.OptionsJson ??= "[]";

            q.CorrectAnswerJson ??= "[]";
        }

        var objectiveQuestions =
            questions.Where(x =>
                x.Type ==
                QuestionType.SingleChoice

                ||

                x.Type ==
                QuestionType.MultipleChoice

                ||

                x.Type ==
                QuestionType.TrueFalse

                ||

                x.Type ==
                QuestionType.Matching

                ||

                x.Type ==
                QuestionType.FillBlank
            ).ToList();

        var essayQuestions =
            questions.Where(x =>
                x.Type ==
                QuestionType.Essay
            ).ToList();
        double totalEssayScore =
            essayQuestions.Count
            * EssayScore;

        if (totalEssayScore > 10)
        {
            totalEssayScore = 10;
        }

        double remainingScore =
            10 - totalEssayScore;

        double objectiveScore = 0;

        if (objectiveQuestions.Count > 0)
        {
            objectiveScore =
                Math.Round(
                    remainingScore /
                    objectiveQuestions.Count,
                    2);
        }

        foreach (var q in objectiveQuestions)
        {
            q.Score = (decimal)objectiveScore;
        }

        foreach (var q in essayQuestions)
        {
            q.Score = (decimal)EssayScore;
        }

        exam.IsPublished = true;

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Tạo đề thi thành công!";

        return RedirectToAction(
            "EditExam",
            new
            {
                id = exam.Id
            });
    }
    private async Task UploadQuestionsInternal(
    int examId,
    IFormFile file)
    {
        var extension = Path
            .GetExtension(file.FileName)
            .ToLower();

        var uploadFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var filePath = Path.Combine(
            uploadFolder,
            Guid.NewGuid() + extension);

        using (var stream = new FileStream(
                   filePath,
                   FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        string text = "";
        if (extension == ".docx")
        {
            using var doc =
                WordprocessingDocument.Open(
                    filePath,
                    false);

            var documentBody =
                doc.MainDocumentPart?
                   .Document?
                   .Body;

            if (documentBody == null)
                return;

            var paragraphTexts =
                new List<string>();

            foreach (var p in documentBody
                .Elements<DocumentFormat
                .OpenXml.Wordprocessing
                .Paragraph>())
            {
                var line =
                    p.InnerText?.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                line = line
                    .Replace("|", "\n")
                    .Replace("=>", "\n=>");

                paragraphTexts.Add(line);
            }

            text = string.Join(
                Environment.NewLine,
                paragraphTexts);

            text = text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
        }
        if (extension == ".json")
        {
            text = await System.IO.File
                .ReadAllTextAsync(filePath);

            var options =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

            var examData =
                JsonSerializer.Deserialize<
                    JsonElement>(
                        text,
                        options);

            if (examData.TryGetProperty(
                    "Questions",
                    out JsonElement questions))
            {
                foreach (var q in questions.EnumerateArray())
                {
                    string content =
                        q.GetProperty("Content")
                         .GetString() ?? "";

                    string type =
                        q.GetProperty("Type")
                         .GetString() ?? "Essay";

                    decimal score =
                        q.GetProperty("Score")
                         .GetDecimal();

                    List<string> optionsList = new();
                    if (q.TryGetProperty("OptionsJson", out JsonElement optionsJson))
                    {
                        string rawOptions =
                            optionsJson.GetString() ?? "[]";

                        optionsList =
                            JsonSerializer.Deserialize<List<string>>(
                                rawOptions
                            ) ?? new List<string>();
                    }
                    List<string> answers = new();
                    if (q.TryGetProperty("CorrectAnswerJson", out JsonElement answersJson))
                    {
                        string rawAnswers =
                            answersJson.GetString() ?? "[]";

                        answers =
                            JsonSerializer.Deserialize<List<string>>(
                                rawAnswers
                            ) ?? new List<string>();
                    }
                    var question =
                        new Question
                        {
                            ExamId = examId,
                            Content = content,
                            Score = score,
                            Type =
                                Enum.Parse<QuestionType>(
                                    type),
                            OptionsJson =
            JsonSerializer.Serialize(optionsList),
                            CorrectAnswerJson =
            JsonSerializer.Serialize(answers)
                        };
                    _context.Questions
                        .Add(question);
                }

                await _context.SaveChangesAsync();

                return;
            }
        }
        else if (extension == ".pdf")
        {
            using var document =
                PdfDocument.Open(filePath);

            var builder = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                var pageText = page.Text;

                if (!string.IsNullOrWhiteSpace(pageText))
                {
                    builder.AppendLine(pageText);
                }
            }

            text = builder.ToString();
            text = text
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");
            text = Regex.Replace(
                text,
                @"[ \t]+",
                " ");
            text = Regex.Replace(
                text,
                @"([A-D]\.)",
                "\n$1");
            text = Regex.Replace(
                text,
                @"(Đáp án đúng:)",
                "\n$1");
            text = Regex.Replace(
                text,
                @"(Essay|Tự luận)",
                "\n$1");
            text = text.Replace("[", "")
                       .Replace("]", "");
            text = Regex.Replace(
                text,
                @"\n{2,}",
                "\n\n");
        }
        else if (extension == ".doc")
        {
            ComponentInfo.SetLicense(
        "FREE-LIMITED-KEY");

            var document =
                DocumentModel.Load(filePath);

            text =
                document.Content.ToString();

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text
                    .Replace("\r\n", "\n")
                    .Replace("\r", "\n")
                    .Trim();
            }
        }

        if (string.IsNullOrWhiteSpace(text))
            return;

        ParseQuestions(text, examId);
    }
    public IActionResult ExamSessionDetails(int id)
    {
        var session = _context.ExamSessions
    .Include(x => x.Exams)
        .ThenInclude(x => x.Questions)
    .FirstOrDefault(x => x.Id == id);

        if (session == null)
            return NotFound();

        return View(session);
    }
    public IActionResult EditExam(int id)
    {
        var exam = _context.Exams
            .Include(e => e.Questions)
            .FirstOrDefault(e => e.Id == id);

        if (exam == null)
        {
            return NotFound();
        }
        foreach (var q in exam.Questions)
        {
            if (string.IsNullOrWhiteSpace(q.OptionsJson))
            {
                q.OptionsJson = "[]";
            }

            if (string.IsNullOrWhiteSpace(q.CorrectAnswerJson))
            {
                q.CorrectAnswerJson = "[]";
            }
        }
        return View(exam);
    }
    [HttpPost]
    public IActionResult EditExam(Exam model)
    {
        var exam = _context.Exams
            .FirstOrDefault(x => x.Id == model.Id);

        if (exam == null)
            return NotFound();

        exam.Title =
            model.Title;

        exam.Duration =
            model.Duration;

        exam.MaxAttempts =
            model.MaxAttempts;

        exam.IsPublished =
            model.IsPublished;

        exam.EnableFullscreen =
            model.EnableFullscreen;

        exam.DetectTabSwitch =
            model.DetectTabSwitch;

        exam.DetectCopyPaste =
            model.DetectCopyPaste;

        exam.DetectRightClick =
            model.DetectRightClick;

        exam.DetectWindowBlur =
            model.DetectWindowBlur;

        exam.DetectDevTools =
            model.DetectDevTools;

        exam.DetectMultiMonitor =
            model.DetectMultiMonitor;

        exam.EnableWebcamMonitoring =
            model.EnableWebcamMonitoring;

        exam.DetectIdle =
            model.DetectIdle;

        exam.DetectAltTab =
            model.DetectAltTab;

        exam.MaxWarnings =
            model.MaxWarnings;

        exam.AutoSubmitWhenCheating =
            model.AutoSubmitWhenCheating;

        exam.LockExamAfterCheating =
            model.LockExamAfterCheating;

        exam.EnableBehaviorAnalytics =
            model.EnableBehaviorAnalytics;
        exam.EnableAIProctoring = model.EnableAIProctoring;
        exam.DetectPhone = model.DetectPhone;
        exam.DetectOtherPerson = model.DetectOtherPerson;
        exam.DetectBooksOrNotes = model.DetectBooksOrNotes;
        exam.FullscreenAlerts = model.FullscreenAlerts;
        exam.ShowCameraBeforeStart = model.ShowCameraBeforeStart;

        var questions = _context.Questions
            .Where(x => x.ExamId == exam.Id)
            .ToList();

        decimal totalScore =
            questions.Sum(q => q.Score);

        if (totalScore != 10)
        {
            TempData["Error"] =
                $"Tổng điểm hiện tại là {totalScore:0.##}. Tổng điểm đề phải đúng bằng 10!";

            return RedirectToAction(
                "EditExam",
                new { id = exam.Id });
        }

        foreach (var q in questions)
        {
            q.Content ??= "";

            q.OptionsJson ??= "[]";

            q.CorrectAnswerJson ??= "[]";
        }

        try
        {
            var hasQuestions = _context.Questions
    .Any(x => x.ExamId == exam.Id);

            if (hasQuestions)
            {
                exam.IsPublished = true;
            }
            else
            {
                exam.IsPublished = false;
            }
            _context.SaveChanges();

            TempData["Success"] =
                "Phân bố đề thi thành công!";
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                ex.InnerException?.Message
                ?? ex.Message;

            return RedirectToAction(
                "EditExam",
                new { id = exam.Id });
        }
        return RedirectToAction(
                "ExamSessionDetails",
                new
                {
                    id = exam.ExamSessionId
                });
    }

    public IActionResult AssignExam(int examId)
    {
        ViewBag.ExamId = examId;

        var students = _context.Users
            .Where(x => x.Role == Role.Student)
            .OrderBy(x => x.Username)
            .ToList();

        return View(students);
    }
    [HttpPost]
    public IActionResult AssignExam(
    int examId,
    List<string> students)
    {
        try
        {
            if (students == null
                || !students.Any())
            {
                TempData["Error"] =
                    "Vui lòng chọn sinh viên.";

                return RedirectToAction(
                    "AssignExam",
                    new { examId });
            }

            var exam =
                _context.Exams
                    .FirstOrDefault(x =>
                        x.Id == examId);

            if (exam == null)
            {
                TempData["Error"] =
                    "Không tìm thấy đề thi.";

                return RedirectToAction(
                    "Index");
            }

            int addedCount = 0;

            foreach (var username in students)
            {
                if (string.IsNullOrWhiteSpace(username))
                    continue;

                string cleanUsername =
                    username.Trim();

                bool exists =
                    _context.AssignedExams.Any(x =>
                        x.ExamId == examId
                        &&
                        x.StudentUsername == cleanUsername);

                if (!exists)
                {
                    _context.AssignedExams.Add(
                        new AssignedExam
                        {
                            ExamId = examId,
                            StudentUsername = cleanUsername
                        });

                    addedCount++;
                }
            }

            _context.SaveChanges();

            TempData["Success"] =
                $"Phân bổ đề thi thành công cho {addedCount} sinh viên.";

            return RedirectToAction(
                "EditExam",
                new { id = examId });
        }
        catch (Exception ex)
        {
            TempData["Error"] =
                ex.InnerException?.Message
                ?? ex.Message;

            return RedirectToAction(
                "AssignExam",
                new { examId });
        }
    }
    private void ParseQuestions(
    string text,
    int examId)
    {
        var lines = text.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.None);

        Question? currentQuestion = null;

        var options =
            new List<string>();

        var correctAnswers =
            new List<string>();

        foreach (var raw in lines)
        {
            var line = raw.Trim();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (
                line.StartsWith("Câu ")
                ||
                line.StartsWith("Question "))
            {
                if (currentQuestion != null)
                {
                    SaveQuestion(
                        currentQuestion,
                        options,
                        correctAnswers,
                        examId);
                }

                currentQuestion =
                    new Question
                    {
                        Content = line
                    };

                options =
                    new List<string>();

                correctAnswers =
                    new List<string>();

                continue;
            }

            if (
                line.StartsWith("A.")
                ||
                line.StartsWith("B.")
                ||
                line.StartsWith("C.")
                ||
                line.StartsWith("D.")
                ||
                line.StartsWith("A)")
                ||
                line.StartsWith("B)")
                ||
                line.StartsWith("C)")
                ||
                line.StartsWith("D)")
                ||
                line.StartsWith("A:")
                ||
                line.StartsWith("B:")
                ||
                line.StartsWith("C:")
                ||
                line.StartsWith("D:")
                ||
                line.StartsWith("A -")
                ||
                line.StartsWith("B -")
                ||
                line.StartsWith("C -")
                ||
                line.StartsWith("D -"))
            {
                options.Add(line);

                continue;
            }

            if (
                line.Equals("Đúng",
                    StringComparison.OrdinalIgnoreCase)
                ||
                line.Equals("Sai",
                    StringComparison.OrdinalIgnoreCase)
                ||
                line.StartsWith("Đúng")
                ||
                line.StartsWith("Sai"))
            {
                options.Add(line);

                continue;
            }

            if (
                line.Contains("->")
                ||
                line.Contains("=>")
                ||
                line.Contains(" - "))
            {
                options.Add(line);

                continue;
            }

            if (
                line.StartsWith("Đáp án")
                ||
                line.StartsWith("DAP AN")
                ||
                line.StartsWith("Answer"))
            {
                var answerLine = line
                    .Replace("Đáp án:", "")
                    .Replace("Đáp án", "")
                    .Replace("DAP AN:", "")
                    .Replace("DAP AN", "")
                    .Replace("Answer:", "")
                    .Replace("Answer", "")
                    .Trim();

                correctAnswers =
                    answerLine
                    .Split(
                        new[] { ',', ';', '/' },
                        StringSplitOptions
                            .RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .ToList();

                continue;
            }

            if (currentQuestion != null)
            {
                currentQuestion.Content +=
                    "\n" + line;
            }
        }

        if (currentQuestion != null)
        {
            SaveQuestion(
                currentQuestion,
                options,
                correctAnswers,
                examId);
        }

        _context.SaveChanges();
    }
    private void SaveQuestion(
    Question q,
    List<string> options,
    List<string> correct,
    int examId)
    {
        QuestionType type =
            QuestionType.SingleChoice;

        if (options.Count == 0 &&
            correct.Count == 0)
        {
            type = QuestionType.Essay;
        }
        else if (correct.Count > 1)
        {
            type = QuestionType.MultipleChoice;
        }
        else if (
            correct.Any(x =>
                x.Equals("Đúng",
                    StringComparison.OrdinalIgnoreCase)
                ||
                x.Equals("Sai",
                    StringComparison.OrdinalIgnoreCase)))
        {
            type = QuestionType.TrueFalse;
        }
        else if (
            q.Content != null &&
            q.Content.Contains("..."))
        {
            type = QuestionType.FillBlank;
        }
        else if (
            options.Any(x =>
                x.Contains(" - ")
                ||
                x.Contains("->")))
        {
            type = QuestionType.Matching;
        }

        var cleanedOptions =
            new List<string>();

        foreach (var op in options)
        {
            if (string.IsNullOrWhiteSpace(op))
                continue;

            var split =
                System.Text.RegularExpressions
                .Regex.Split(
                    op,
                    @"(?=[A-D][\.\:\)\-])")
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList();

            if (split.Count > 1)
            {
                cleanedOptions.AddRange(split);
            }
            else
            {
                cleanedOptions.Add(op.Trim());
            }
        }

        var cleanedAnswers =
            new List<string>();

        foreach (var ans in correct)
        {
            if (string.IsNullOrWhiteSpace(ans))
                continue;

            string answer =
                ans.Trim();

            if (answer.Equals("A",
                StringComparison.OrdinalIgnoreCase)
                &&
                cleanedOptions.Count > 0)
            {
                answer =
                    cleanedOptions[0];
            }
            else if (
                answer.Equals("B",
                StringComparison.OrdinalIgnoreCase)
                &&
                cleanedOptions.Count > 1)
            {
                answer =
                    cleanedOptions[1];
            }
            else if (
                answer.Equals("C",
                StringComparison.OrdinalIgnoreCase)
                &&
                cleanedOptions.Count > 2)
            {
                answer =
                    cleanedOptions[2];
            }
            else if (
                answer.Equals("D",
                StringComparison.OrdinalIgnoreCase)
                &&
                cleanedOptions.Count > 3)
            {
                answer =
                    cleanedOptions[3];
            }

            cleanedAnswers.Add(answer);
        }

        if (!cleanedAnswers.Any()
            &&
            type != QuestionType.Essay)
        {
            cleanedAnswers =
                new List<string>();
        }

        _context.Questions.Add(
            new Question
            {
                Content =
                    q.Content ?? "",

                Type = type,

                OptionsJson =
                    JsonSerializer.Serialize(
                        cleanedOptions),

                CorrectAnswerJson =
                    JsonSerializer.Serialize(
                        cleanedAnswers),

                Score = 0,

                ExamId = examId
            });
    }
    public IActionResult EssayGrading(int examId)
    {
        var results = _context.ExamResults
            .Include(x => x.Student)
            .Include(x => x.Exam!)
                .ThenInclude(x => x.Questions)
            .Where(x => x.ExamId == examId)
            .ToList();

        return View(results);
    }
    [HttpPost]
    public IActionResult DeleteQuestion(int id)
    {
        var q = _context.Questions.Find(id);
        if (q != null)
        {
            int examId = q.ExamId;
            _context.Questions.Remove(q);
            _context.SaveChanges();
            return RedirectToAction("EditExam", new { id = examId });
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult EditQuestion(Question model)
    {
        var q = _context.Questions.Find(model.Id);
        if (q == null) return NotFound();

        q.Content = model.Content;
        q.OptionsJson = model.OptionsJson;
        q.CorrectAnswerJson = model.CorrectAnswerJson;
        q.Type = model.Type;

        _context.SaveChanges();

        return RedirectToAction("EditExam", new { id = q.ExamId });
    }
    public IActionResult PreviewExam(int id)
    {
        var exam = _context.Exams
            .Include(x => x.Questions)
            .FirstOrDefault(x => x.Id == id);
        if (exam == null) return NotFound();
        exam.Questions = exam.Questions
            .OrderBy(q => q.Order)
            .ToList();
        return View(exam);
    }
    public IActionResult AIReport(int examId)
    {
        var logs = _context.AIProctoringLogs
            .Where(x => x.ExamId == examId)
            .OrderByDescending(x => x.TimeDetected)
            .ToList();
        return View(logs);
    }
    [HttpPost]
    public IActionResult UpdateEssayScore(
    int id,
    decimal score,
    string? teacherComment)
    {
        var result =
            _context.ExamResults
            .FirstOrDefault(x =>
                x.Id == id);

        if (result == null)
            return NotFound();

        result.EssayScore =
            score;

        result.TeacherComment =
            teacherComment;

        result.IsEssayGraded =
            true;

        _context.SaveChanges();

        TempData["Success"] =
            "Đã cập nhật điểm tự luận.";

        return RedirectToAction(
            "EssayGrading");
    }
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var session = _context.ExamSessions
            .Include(x => x.Exams)
            .FirstOrDefault(x => x.Id == id);
        if (session == null)
            return NotFound();
        if (session.Exams != null && session.Exams.Any())
        {
            _context.Exams.RemoveRange(session.Exams);
        }
        _context.ExamSessions.Remove(session);
        _context.SaveChanges();
        TempData["Success"] = "Xóa kỳ thi thành công!";
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteExam(int id)
    {
        var exam = _context.Exams
            .Include(x => x.Questions)
            .FirstOrDefault(x => x.Id == id);
        if (exam == null)
        {
            TempData["Error"] =
                "Không tìm thấy đề thi!";

            return RedirectToAction("Index");
        }
        var examSessionId =
            exam.ExamSessionId;
        if (exam.Questions != null &&
            exam.Questions.Any())
        {
            _context.Questions
                .RemoveRange(exam.Questions);
        }
        _context.Exams.Remove(exam);

        await _context.SaveChangesAsync();

        TempData["Success"] =
            "Xóa đề thi thành công!";

        return RedirectToAction(
            "ExamSessionDetails",
            new { id = examSessionId });
    }
}