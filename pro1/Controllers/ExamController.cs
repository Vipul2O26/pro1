using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using pro1.Data;
using pro1.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
            var userIdClaim = User.FindFirst("UserID")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId) || userId == 0)
            {
                TempData["Error"] = "User not authenticated.";
                return RedirectToAction("Login", "Account");
            }

            // Use the database context directly and perform the grouping in the database query.
            var uniqueSubjects = _context.SubjectUnits
                .Where(s => s.CreatedByUserID == userId)
                .GroupBy(s => new
                {
                    SubjectName = s.SubjectName.Trim().ToLower(), // Trim and lowercase in the query
                    SubjectCode = s.SubjectCode.Trim().ToLower(), // Trim and lowercase in the query
                    s.Semester
                })
                .Select(g => g.FirstOrDefault()) // Use FirstOrDefault() in case a group is empty.
                .ToList();

            return View(uniqueSubjects);
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
        public IActionResult GenerateExam([FromForm] string subjectName, [FromForm] Dictionary<string, int> unitQuestions, int durationTime) // Added durationTime parameter
        {
            var selectedQuestions = new List<MCQQuestion>();

            // Fetch facultyId from session
            int? facultyId = HttpContext.Session.GetInt32("UserID");
            if (facultyId == null)
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            // Loop through each unit to select questions dynamically
            foreach (var entry in unitQuestions)
            {
                var unitName = entry.Key;
                var count = entry.Value;

                if (count > 0)
                {
                    var questions = _context.MCQQuestions
                        .Where(q => q.SubjectUnit.SubjectName == subjectName && q.SubjectUnit.UnitName == unitName)
                        .OrderBy(q => Guid.NewGuid()) // Random order
                        .Take(count)
                        .ToList();

                    selectedQuestions.AddRange(questions);
                }
            }

            // Store data in session.  Consider not using session for the questions.
            HttpContext.Session.SetString("SubjectName", subjectName);
            HttpContext.Session.SetString("MCQs", JsonConvert.SerializeObject(selectedQuestions));


            // Log for debugging
            if (selectedQuestions.Count == 0)
            {
                TempData["Error"] = "No questions selected or available.";
                return RedirectToAction("Create", new { subjectName = subjectName });
            }

            // Create and save the exam to the database
            var subjectUnit = _context.SubjectUnits.FirstOrDefault(su => su.SubjectName == subjectName);
            if (subjectUnit == null)
            {
                TempData["Error"] = "Subject Unit not found.";
                return RedirectToAction("Index");
            }

            var exam = new Exams
            {
                FacultyID = facultyId.Value,
                SubjectUnitID = subjectUnit.ID,
                SubjectCode = subjectUnit.SubjectCode,
                TotalQuestions = selectedQuestions.Count,
                DurationTime = durationTime, // Use the value from the form
                CreatedAt = DateTime.Now
            };

            try
            {
                _context.Exams.Add(exam);
                _context.SaveChanges();

                // Save the questions related to the exam
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
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error while saving exam: " + ex.Message;
                Console.Error.WriteLine(ex);
            }


            return RedirectToAction("ExamDetails", new { id = exam.ExamID });
        }



        [HttpPost]
        [Authorize(Roles = "Faculty")]
        public IActionResult SaveGeneratedExam(int subjectUnitId, int durationMinutes)
        {
            // Fetch FacultyID from session
            int? facultyId = HttpContext.Session.GetInt32("UserID");
            if (facultyId == null)
            {
                TempData["Error"] = "Session expired. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            // Fetch questions from session
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
                return RedirectToAction("Index");
            }

            var subjectUnit = _context.SubjectUnits.FirstOrDefault(su => su.ID == subjectUnitId);
            if (subjectUnit == null)
            {
                TempData["Error"] = "Subject Unit not found.";
                return RedirectToAction("Index");
            }

            // Create and save the exam
            var exam = new Exams
            {
                FacultyID = facultyId.Value,
                SubjectUnitID = subjectUnitId,
                SubjectCode = subjectUnit.SubjectCode,
                TotalQuestions = selectedQuestions.Count,
                DurationTime = durationMinutes,
                CreatedAt = DateTime.Now
            };

            _context.Exams.Add(exam);
            _context.SaveChanges();

            // Save the questions related to the exam
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
                .Include(e => e.SubjectUnit)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.McqQuestion)
                .FirstOrDefault(e => e.ExamID == id);

            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        [Authorize(Roles = "Faculty")]
        public IActionResult ExamCreated()
        {
            int facultyId = int.Parse(User.FindFirst("UserID").Value);

            var exams = _context.Exams
                .Where(e => e.FacultyID == facultyId)
                .Include(e => e.SubjectUnit)
                .ToList();

            if (exams == null || exams.Count == 0)
            {
                ViewBag.Message = "No exams found for Faculty ID: " + facultyId;
            }
            else
            {
                ViewBag.Message = "Total Exams Found: " + exams.Count;
            }

            return View(exams);
        }

        public IActionResult ExportCsv(int id, bool withAnswers)
        {
            var exam = _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.McqQuestion)
                .FirstOrDefault(e => e.ExamID == id);

            if (exam == null)
            {
                return NotFound();
            }

            var sb = new StringBuilder();
            sb.AppendLine("Question,Option A,Option B,Option C,Option D" + (withAnswers ? ",Correct Answer" : ""));

            foreach (var eq in exam.ExamQuestions)
            {
                var q = eq.McqQuestion;
                sb.AppendLine($"{q.QuestionText},{q.OptionA},{q.OptionB},{q.OptionC},{q.OptionD}" + (withAnswers ? $",{q.CorrectAnswer}" : ""));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"Exam_{exam.ExamID}.csv");
        }

        public IActionResult ExportPdf(int id, bool withAnswers)
        {
            var exam = _context.Exams
                .Include(e => e.ExamQuestions)
                    .ThenInclude(eq => eq.McqQuestion)
                .FirstOrDefault(e => e.ExamID == id);

            if (exam == null)
            {
                return NotFound();
            }

            using (var stream = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                var questionFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);

                doc.Add(new Paragraph("Exam ID: " + exam.ExamID, titleFont));
                doc.Add(new Paragraph("\n"));

                foreach (var eq in exam.ExamQuestions)
                {
                    var q = eq.McqQuestion;
                    doc.Add(new Paragraph($"Q: {q.QuestionText}", questionFont));
                    doc.Add(new Paragraph($"A) {q.OptionA}", questionFont));
                    doc.Add(new Paragraph($"B) {q.OptionB}", questionFont));
                    doc.Add(new Paragraph($"C) {q.OptionC}", questionFont));
                    doc.Add(new Paragraph($"D) {q.OptionD}", questionFont));

                    if (withAnswers)
                    {
                        doc.Add(new Paragraph($"Correct Answer: {q.CorrectAnswer}", questionFont));
                    }

                    doc.Add(new Paragraph("\n"));
                }

                doc.Close();
                return File(stream.ToArray(), "application/pdf", $"Exam_{exam.ExamID}.pdf");
            }
        }
    }
}
