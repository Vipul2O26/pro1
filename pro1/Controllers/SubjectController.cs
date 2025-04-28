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

            var subjects = _context.SubjectUnits
                .Where(su => su.CreatedByUserID == userId)  // filter by logged-in user
                .ToList();

            return View(subjects);
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

                    // Create SubjectUnit from ViewModel
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

            // Important: Pass a SubjectUnit (not ViewModel) if you return View
            return View(new SubjectUnit
            {
                SubjectName = model.SubjectName,
                Semester = model.Semester,
                SubjectCode = model.SubjectCode,
                UnitName = model.UnitName
            });
        }

    }
}
