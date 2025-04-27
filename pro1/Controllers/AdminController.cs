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
                .ToList(); 

            return View(logs);
        }


        public IActionResult AllUsers()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        public IActionResult Edit(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);  
        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public IActionResult Edit(int id, User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(u => u.UserID == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllUsers));
            }
            return View(user);
        }



        public IActionResult Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user); // Show delete confirmation view
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return RedirectToAction(nameof(AllUsers)); // Redirect back to the list page after deletion
        }


    }
}
