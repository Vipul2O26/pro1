using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pro1.Data;
using pro1.Models;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace pro1.Controllers
{
    public class ExamController : Controller
    {
        private readonly AppDbContext _context;

        public ExamController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get unique subject combinations (by SubjectName and SubjectCode)
            var subjects = _context.SubjectUnits
                .GroupBy(s => new { s.SubjectName, s.SubjectCode, s.Semester })
                .Select(g => g.First())
                .ToList();

            return View(subjects);
        }
        public IActionResult Create(string subjectName)
        {
            var subjectUnits = _context.SubjectUnits
                .Where(s => s.SubjectName == subjectName)
                .ToList();

            ViewBag.SubjectName = subjectName;
            return View(subjectUnits);
        }

        [HttpPost]
        public IActionResult GenerateExam([FromForm] string SubjectName, [FromForm] Dictionary<string, int> UnitQuestions)
        {
            var selectedQuestions = new List<MCQQuestion>();

            foreach (var entry in UnitQuestions)
            {
                var unitName = entry.Key;
                var count = entry.Value;

                if (count > 0)
                {
                    var questions = _context.MCQQuestions
                        .Where(q => q.SubjectUnit.UnitName == unitName && q.SubjectUnit.SubjectName == SubjectName)
                        .OrderBy(q => Guid.NewGuid())
                        .Take(count)
                        .ToList();

                    selectedQuestions.AddRange(questions);
                }
            }

            // Store in session for later download
            HttpContext.Session.SetString("SubjectName", SubjectName);
            HttpContext.Session.SetString("MCQs", JsonConvert.SerializeObject(selectedQuestions));

            return View("ExamPreview", selectedQuestions);
        }

        [HttpPost]
        public IActionResult DownloadExamPDF()
        {
            string json = HttpContext.Session.GetString("MCQs");
            var questions = JsonConvert.DeserializeObject<List<MCQQuestion>>(json);

            using (var stream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                var writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var font = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                doc.Add(new Paragraph("MCQ Exam", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));

                foreach (var q in questions)
                {
                    doc.Add(new Paragraph($"Q: {q.QuestionText}", font));
                    doc.Add(new Paragraph($"A. {q.OptionA}", font));
                    doc.Add(new Paragraph($"B. {q.OptionB}", font));
                    doc.Add(new Paragraph($"C. {q.OptionC}", font));
                    doc.Add(new Paragraph($"D. {q.OptionD}", font));
                    doc.Add(new Paragraph($"Answer: {q.CorrectAnswer}", font));
                    doc.Add(new Paragraph("\n"));
                }

                doc.Close();
                byte[] pdfBytes = stream.ToArray();
                return File(pdfBytes, "application/pdf", "Exam.pdf");
            }
        }

        [HttpPost]
        public IActionResult SaveGeneratedExam(int facultyId, int subjectUnitId, int durationMinutes)
        {
            string json = HttpContext.Session.GetString("MCQs");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "Session expired or no questions found.";
                return RedirectToAction("Index");
            }

            var selectedQuestions = JsonConvert.DeserializeObject<List<MCQQuestion>>(json);

            if (selectedQuestions == null || selectedQuestions.Count == 0)
            {
                TempData["Error"] = "No questions selected.";
                return RedirectToAction("GenerateExam");
            }

            var exam = new Exams
            {
                FacultyID = facultyId,
                SubjectUnitID = subjectUnitId,
                TotalQuestions = selectedQuestions.Count,
                DurationTime = durationMinutes,
                CreatedAt = DateTime.Now
            };

            _context.Exams.Add(exam);
            _context.SaveChanges();

            foreach (var mcq in selectedQuestions)
            {
                var examQuestion = new ExamQuestion
                {
                    ExamID = exam.ExamID,
                    QuestionID = mcq.QuestionID
                };
                _context.ExamQuestions.Add(examQuestion);
            }

            _context.SaveChanges();

            TempData["Success"] = "Exam and questions saved successfully!";
            return RedirectToAction("ExamDetails", new { id = exam.ExamID });
        }

        public IActionResult ExamDetails(int id)
        {
            var exam = _context.Exams
                .Where(e => e.ExamID == id)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.McqQuestion)
                .FirstOrDefault();

            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }



    }
}
