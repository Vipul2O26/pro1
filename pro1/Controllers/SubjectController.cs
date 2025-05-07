using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pro1.Data;
using pro1.Models;
using System.Security.Claims;

namespace pro1.Controllers
{
    public class SubjectController : Controller
    {
        private readonly AppDbContext _context;

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirst("UserID").Value);

            // Get distinct subjects created by the user
            var subjects = _context.SubjectUnits
                .Where(su => su.CreatedByUserID == userId)
                .GroupBy(su => new { su.SubjectName, su.SubjectCode, su.Semester })
                .Select(g => new
                {
                    g.Key.SubjectName,
                    g.Key.SubjectCode,
                    g.Key.Semester
                })
                .ToList();

            return View(subjects);
        }

        public IActionResult ViewUnits(string subject, string code)
        {
            int userId = int.Parse(User.FindFirst("UserID").Value);

            var units = _context.SubjectUnits
                .Where(su => su.CreatedByUserID == userId && su.SubjectName == subject && su.SubjectCode == code)
                .ToList();

            ViewBag.SubjectTitle = subject;
            return View(units);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SubjectUnitViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userIdClaim = User.FindFirst("UserID");
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);

                    // ✅ Check if subject-unit combination already exists
                    bool isDuplicate = await _context.SubjectUnits
                        .AnyAsync(su => su.SubjectName == model.SubjectName && su.UnitName == model.UnitName);

                    if (isDuplicate)
                    {
                        ModelState.AddModelError(string.Empty, "This subject and unit are already exists.");
                        return View(model);
                    }

                    // Create and add new SubjectUnit
                    var entity = new SubjectUnit
                    {
                        SubjectName = model.SubjectName,
                        Semester = model.Semester,
                        SubjectCode = model.SubjectCode,
                        UnitName = model.UnitName,
                        CreatedByUserID = userId
                    };

                    _context.SubjectUnits.Add(entity);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Subject unit added successfully!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Session expired. Please login again.";
                    return RedirectToAction("Login", "Account");
                }
            }

            return View(model);

        }


    }
}
