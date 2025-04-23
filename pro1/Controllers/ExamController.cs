using Microsoft.AspNetCore.Mvc;
using pro1.Data;
using pro1.Models;
using System.Linq;

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
                        .OrderBy(q => Guid.NewGuid()) // random
                        .Take(count)
                        .ToList();

                    selectedQuestions.AddRange(questions);
                }
            }

            // You can now display or save selectedQuestions
            return View("ExamPreview", selectedQuestions); // You can create ExamPreview.cshtml to show them
        }



    }
}
