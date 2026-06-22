using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Data;
using OnlineExamSystem.Models;
using OnlineExamSystem.Models.Enums;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Text.Json;

namespace OnlineExamSystem.Controllers
{
    public class QuestionBankController
        : Controller
    {
        private readonly
            OnlineExamSystemContext _context;

        public QuestionBankController(
            OnlineExamSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult>
        Index(
            string? search,
            QuestionType? type)
        {
            var query =
                _context.QuestionBanks
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Content.Contains(search));
            }

            if (type != null)
            {
                query = query.Where(x =>
                    x.Type == type);
            }

            var data =
                await query
                .OrderByDescending(x => x.Id)
                .Take(300)
                .ToListAsync();

            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>
        Create(
            QuestionBank model,
            List<string>? options,
            List<string>? answers,
            IFormFile? audioFile)
        {
            if (model == null)
            {
                TempData["Error"] = "Dữ liệu gửi lên không hợp lệ.";
                return View(new QuestionBank());
            }
            if (string.IsNullOrWhiteSpace(model.Content))
            {
                TempData["Error"] =
                    "Vui lòng nhập nội dung câu hỏi!";

                return View(model);
            }

            model.Subject ??= "";

            model.Difficulty ??= "";

            options ??= new List<string>();

            answers ??= new List<string>();

            options = options
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .ToList();

            answers = answers
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .ToList();

            model.OptionsJson =
                JsonSerializer.Serialize(options);

            model.CorrectAnswerJson =
                JsonSerializer.Serialize(answers);

            try
            {
                _context.QuestionBanks.Add(model);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Thêm câu hỏi thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message
                    ?? ex.Message;

                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult>
        Edit(int id)
        {
            var q =
                await _context.QuestionBanks
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (q == null)
                return NotFound();

            return View(q);
        }

        [HttpPost]
        public async Task<IActionResult>
Edit(
    QuestionBank model,
    List<string>? options,
    List<string>? answers,
    IFormFile? audioFile)
        {
            var q =
                await _context.QuestionBanks
                .FirstOrDefaultAsync(x =>
                    x.Id == model.Id);

            if (q == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(
                model.Content?.Trim()))
            {
                TempData["Error"] =
                    "Vui lòng nhập nội dung câu hỏi!";

                return View(model);
            }

            q.Content =
                model.Content.Trim();

            q.Type =
                model.Type;

            q.Subject =
                model.Subject ?? "";

            q.Difficulty =
                model.Difficulty ?? "";

            q.Score =
                model.Score;

            options ??= new();

            answers ??= new();
            options =
                options
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                    x.Trim())
                .Distinct()
                .ToList();
            answers =
                answers
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Select(x =>
                    x.Trim())
                .Distinct()
                .ToList();
            if (q.Type == QuestionType.SingleChoice ||
                q.Type == QuestionType.MultipleChoice)
            {
                answers =
                    answers
                    .Where(a =>
                        options.Contains(a))
                    .ToList();
            }

            q.OptionsJson =
                JsonSerializer.Serialize(options);

            q.CorrectAnswerJson =
                JsonSerializer.Serialize(answers);

            try
            {
                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Cập nhật thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message
                    ?? ex.Message;

                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult>
        Delete(int id)
        {
            var q =
                await _context.QuestionBanks
                .FirstOrDefaultAsync(x =>
                    x.Id == id);

            if (q == null)
                return NotFound();

            _context.QuestionBanks
                .Remove(q);

            await _context
                .SaveChangesAsync();

            TempData["Success"] =
                "Đã xóa câu hỏi!";

            return RedirectToAction(
                "Index");
        }

        public async Task<IActionResult>
        ExportWord()
        {
            var questions =
                await _context.QuestionBanks
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();

            using var stream =
                new MemoryStream();

            using (var document =
                WordprocessingDocument.Create(
                    stream,
                    DocumentFormat.OpenXml
                    .WordprocessingDocumentType
                    .Document,
                    true))
            {
                var mainPart =
                    document.AddMainDocumentPart();

                mainPart.Document =
    new DocumentFormat.OpenXml.Wordprocessing.Document();

                var body =
                    new Body();

                int index = 1;

                foreach (var q in questions)
                {
                    body.Append(
                        new Paragraph(
                            new Run(
                                new Text(
                                    $"Câu {index}: {q.Content}"))));

                    body.Append(
                        new Paragraph(
                            new Run(
                                new Text(
                                    $"Loại: {q.Type}"))));

                    var options =
                        string.IsNullOrWhiteSpace(
                            q.OptionsJson)
                        ? new List<string>()
                        : JsonSerializer
                            .Deserialize<List<string>>(
                                q.OptionsJson)
                          ?? new();

                    var answers =
                        string.IsNullOrWhiteSpace(
                            q.CorrectAnswerJson)
                        ? new List<string>()
                        : JsonSerializer
                            .Deserialize<List<string>>(
                                q.CorrectAnswerJson)
                          ?? new();

                    if (
                        q.Type ==
                        QuestionType.SingleChoice

                        ||

                        q.Type ==
                        QuestionType.MultipleChoice)
                    {
                        string[] letters =
                        {
                            "A",
                            "B",
                            "C",
                            "D"
                        };

                        for (
                            int i = 0;
                            i < options.Count;
                            i++)
                        {
                            bool correct =
                                answers.Contains(
                                    options[i]);

                            body.Append(
                                new Paragraph(
                                    new Run(
                                        new Text(
                                            $"{letters[i]}. " +
                                            $"{options[i]}" +
                                            $"{(correct ? " ✓" : "")}"))));
                        }
                    }
                    else if (
                        q.Type ==
                        QuestionType.TrueFalse)
                    {
                        body.Append(
                            new Paragraph(
                                new Run(
                                    new Text(
                                        "Đáp án: " +
                                        string.Join(
                                            ", ",
                                            answers)))));
                    }
                    else if (
                        q.Type ==
                        QuestionType.Matching)
                    {
                        body.Append(
                            new Paragraph(
                                new Run(
                                    new Text(
                                        "Ghép cặp:"))));

                        foreach (var op in options)
                        {
                            body.Append(
                                new Paragraph(
                                    new Run(
                                        new Text(op))));
                        }

                        body.Append(
                            new Paragraph(
                                new Run(
                                    new Text(
                                        "Đáp án: " +
                                        string.Join(
                                            ", ",
                                            answers)))));
                    }
                    else
                    {
                        body.Append(
                            new Paragraph(
                                new Run(
                                    new Text(
                                        "Đáp án: " +
                                        string.Join(
                                            ", ",
                                            answers)))));
                    }

                    body.Append(
                        new Paragraph(
                            new Run(
                                new Text(
                                    "--------------------------------"))));

                    index++;
                }

                mainPart.Document
                    .Append(body);

                mainPart.Document
                    .Save();
            }

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "NganHangCauHoi.docx");
        }

        public async Task<IActionResult>
        ExportPdf()
        {
            var questions =
                await _context.QuestionBanks
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();

            QuestPDF.Settings.License =
                LicenseType.Community;

            var document =
                QuestPDF.Fluent.Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(20);

                        page.Header()
                            .Text(
                                "NGÂN HÀNG CÂU HỎI")
                            .FontSize(20)
                            .Bold();

                        page.Content()
                            .Column(col =>
                            {
                                int index = 1;

                                foreach (var q in questions)
                                {
                                    col.Item()
                                        .Text(
                                            $"Câu {index}: {q.Content}")
                                        .Bold();

                                    col.Item()
                                        .Text(
                                            $"Loại: {q.Type}");

                                    var options =
                                        string.IsNullOrWhiteSpace(
                                            q.OptionsJson)
                                        ? new List<string>()
                                        : JsonSerializer
                                            .Deserialize<List<string>>(
                                                q.OptionsJson)
                                          ?? new();

                                    var answers =
                                        string.IsNullOrWhiteSpace(
                                            q.CorrectAnswerJson)
                                        ? new List<string>()
                                        : JsonSerializer
                                            .Deserialize<List<string>>(
                                                q.CorrectAnswerJson)
                                          ?? new();

                                    if (
                                        q.Type ==
                                        QuestionType.SingleChoice

                                        ||

                                        q.Type ==
                                        QuestionType.MultipleChoice)
                                    {
                                        string[] letters =
                                        {
                                            "A",
                                            "B",
                                            "C",
                                            "D"
                                        };

                                        for (
                                            int i = 0;
                                            i < options.Count;
                                            i++)
                                        {
                                            bool correct =
                                                answers.Contains(
                                                    options[i]);

                                            col.Item()
                                                .Text(
                                                    $"{letters[i]}. " +
                                                    $"{options[i]}" +
                                                    $"{(correct ? " ✓" : "")}");
                                        }
                                    }
                                    else if (
                                        q.Type ==
                                        QuestionType.TrueFalse)
                                    {
                                        col.Item()
                                            .Text(
                                                "Đáp án: " +
                                                string.Join(
                                                    ", ",
                                                    answers));
                                    }
                                    else if (
                                        q.Type ==
                                        QuestionType.Matching)
                                    {
                                        col.Item()
                                            .Text(
                                                "Ghép cặp:");

                                        foreach (var op in options)
                                        {
                                            col.Item()
                                                .Text(op);
                                        }

                                        col.Item()
                                            .Text(
                                                "Đáp án: " +
                                                string.Join(
                                                    ", ",
                                                    answers));
                                    }
                                    else
                                    {
                                        col.Item()
                                            .Text(
                                                "Đáp án: " +
                                                string.Join(
                                                    ", ",
                                                    answers));
                                    }

                                    col.Item()
                                        .PaddingBottom(15);

                                    index++;
                                }
                            });
                    });
                });

            var pdf =
                document.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                "NganHangCauHoi.pdf");
        }
    }
}