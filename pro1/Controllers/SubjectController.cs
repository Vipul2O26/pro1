using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pro1.Data;
using pro1.Models;

namespace pro1.Controllers
{
    public class SubjectController : Controller
    {
        private readonly AppDbContext _context;

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var units = await _context.SubjectUnits.ToListAsync();
            return View(units);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectUnitViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubjectUnit
                {
                    SubjectName = model.SubjectName,
                    Semester = model.Semester,
                    SubjectCode = model.SubjectCode,
                    UnitName = model.UnitName
                };

                _context.SubjectUnits.Add(entity);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Subject unit added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


    }
}
