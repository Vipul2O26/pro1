using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pro1.Data;
using pro1.Models;
using System.Linq;

namespace pro1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            ViewBag.Name = HttpContext.Session.GetString("FullName");
            return View();
        }

        public IActionResult Log()
        {
            var logs = _context.Audits
                .Include(a => a.User)
                .OrderByDescending(a => a.Login_time)
                .ToList(); // Return full Audit model

            return View(logs);
        }


        public IActionResult AllUsers()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

    }
}
