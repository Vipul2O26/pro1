using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pro1.Data;
using pro1.Models;
using pro1.ViewModels;
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

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserID"));
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

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

            var viewModel = new EditUserViewModel
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.UserID == model.UserID);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;

            _context.SaveChanges();

            TempData["Success"] = "User updated successfully!";
            return RedirectToAction("AllUsers", "Admin");
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
