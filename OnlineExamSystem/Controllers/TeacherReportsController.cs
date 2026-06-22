using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineExamSystem.Data;
using OnlineExamSystem.Models;
using OnlineExamSystem.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace OnlineExamSystem.Controllers
{
    [Route("TeacherReports")]
    public class TeacherReportsController : Controller
    {
        private readonly OnlineExamSystemContext _context;

        public TeacherReportsController(
            OnlineExamSystemContext context)
        {
            _context = context;
        }
        [HttpGet("ExamSessionReport")]
        public async Task<IActionResult> ExamSessionReport(
    int examSessionId)
        {
            try
            {
                var session = await _context.ExamSessions
                    .Include(x => x.ExamResults)
                    .ThenInclude(x => x.Student)
                    .Include(x => x.Exams)
                    .FirstOrDefaultAsync(x =>
                        x.Id == examSessionId);

                if (session == null)
                {
                    return Content(
                        "Không tìm thấy kỳ thi");
                }

                var results = session.ExamResults != null
                    ? session.ExamResults
                        .OrderByDescending(x => x.Score)
                        .ToList()
                    : new List<ExamResult>();

                var model = new ExamReportViewModel
                {
                    ExamSessionId = session.Id,

                    SessionName =
                        session.Name ?? "",

                    ExamTitle =
                        session.Exams
                            .FirstOrDefault()?.Title ?? "",

                    TotalStudents = results
                        .Select(x => x.StudentId)
                        .Distinct()
                        .Count(),

                    TotalAttempts =
                        results.Count,

                    AverageScore =
                        results.Any()
                            ? results.Average(x => x.Score)
                            : 0,

                    HighestScore =
                        results.Any()
                            ? results.Max(x => x.Score)
                            : 0,

                    LowestScore =
                        results.Any()
                            ? results.Min(x => x.Score)
                            : 0,

                    TotalViolations = 0,

                    TotalAiViolations = 0,

                    TotalCopyPasteViolations = 0,

                    ExcellentCount =
                        results.Count(x =>
                            x.Score >= 8m),

                    GoodCount =
                        results.Count(x =>
                            x.Score >= 6.5m &&
                            x.Score < 8m),

                    AverageCount =
                        results.Count(x =>
                            x.Score >= 5m &&
                            x.Score < 6.5m),

                    WeakCount =
                        results.Count(x =>
                            x.Score < 5m),

                    Results = results
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel(
            int examSessionId)
        {
            var results = await _context.Results
                .Include(x => x.User)
                .Where(x =>
                    x.ExamSessionId ==
                    examSessionId)
                .OrderByDescending(x =>
                    x.Score)
                .ToListAsync();

            using var workbook =
                new XLWorkbook();

            var worksheet =
                workbook.Worksheets
                    .Add("BaoCao");

            worksheet.Cell(1, 1).Value = "STT";
            worksheet.Cell(1, 2).Value = "Học sinh";
            worksheet.Cell(1, 3).Value = "Điểm";
            worksheet.Cell(1, 4).Value = "Vi phạm";

            int row = 2;

            foreach (var item in results)
            {
                worksheet.Cell(row, 1)
                    .Value = row - 1;

                worksheet.Cell(row, 2)
                    .Value =
                        item.User?.Username;

                worksheet.Cell(row, 3)
                    .Value =
                        item.Score;

                worksheet.Cell(row, 4)
                    .Value =
                        item.ViolationCount;

                row++;
            }

            worksheet.Columns()
                .AdjustToContents();

            using var stream =
                new MemoryStream();

            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "BaoCaoKyThi.xlsx");
        }
        [HttpGet("ExportPdf")]
        public async Task<IActionResult> ExportPdf(
            int examSessionId)
        {
            var results = await _context.Results
                .Include(x => x.User)
                .Where(x =>
                    x.ExamSessionId ==
                    examSessionId)
                .ToListAsync();

            QuestPDF.Settings.License =
                LicenseType.Community;

            var document =
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(20);

                        page.Header()
                            .Text(
                                "BÁO CÁO KẾT QUẢ")
                            .FontSize(20)
                            .Bold();

                        page.Content()
                            .Column(column =>
                            {
                                int stt = 1;

                                foreach (var item in results)
                                {
                                    column.Item()
                                        .Text(
                                            stt + ". " +
                                            item.User?.Username +
                                            " - Điểm: " +
                                            item.Score);

                                    stt++;
                                }
                            });
                    });
                });

            var pdf =
                document.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                "BaoCaoKyThi.pdf");
        }
    }
}