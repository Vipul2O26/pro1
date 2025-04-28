using Microsoft.AspNetCore.Mvc;
using pro1.Data;
using pro1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using pro1.Helper;
using pro1.ViewModels;



namespace pro1.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = model.Password, // Hashing recommended
                    Role = model.Role
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }

            return View(model);
        }


        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Store user data in session
                HttpContext.Session.SetInt32("UserID", user.UserID);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("Role", user.Role);

                // Log login time
                int auditId = AuditHelper.LogLogin(_context, user.UserID);
                HttpContext.Session.SetInt32("AuditID", auditId);

                // Setup identity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect based on role
                return RedirectToAction("Dashboard", user.Role);
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            int? auditId = HttpContext.Session.GetInt32("AuditID");

            if (auditId.HasValue)
            {
                AuditHelper.LogLogout(_context, auditId.Value);
            }

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

        // GET
        public IActionResult EditProfile(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserEditViewModel
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // TEMP: See what errors happen
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = string.Join("<br>", errors);
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.UserID == model.UserID);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
           

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.Password = model.NewPassword;
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Dashboard", "Faculty");
        }




    }
}
