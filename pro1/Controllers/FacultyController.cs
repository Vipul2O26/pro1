﻿using Microsoft.AspNetCore.Mvc;
using pro1.Data;
using pro1.Models;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;



namespace pro1.Controllers
{
    public class FacultyController : Controller
    {
        private readonly AppDbContext _context;

        public FacultyController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserID"));
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (!IsUserLoggedIn())
                return RedirectToAction("Login", "Account");

            ViewBag.Name = HttpContext.Session.GetString("FullName");
            ViewBag.SuccessMessage = TempData["Success"] as string;
            return View();
        }
        [HttpGet]
        public IActionResult UploadMCQs()
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdClaim.Value);

            // Filter subject units by current user
            var userSubjectUnits = _context.SubjectUnits
                .Where(su => su.CreatedByUserID == userId)
                .ToList();

            ViewBag.SubjectUnits = userSubjectUnits;

            // Get distinct semesters from the user's subject units
            ViewBag.Semesters = userSubjectUnits
                .Select(su => su.Semester)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            return View();
        }


        [HttpPost]
        public IActionResult UploadMCQs(IFormFile file, int SubjectUnitID)
        {
            if (file != null && file.Length > 0)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<MCQQuestionMap>(); // Use your custom mapping

                    var mcqs = csv.GetRecords<MCQQuestion>().ToList();

                    // Set SubjectUnitID for each record (not coming from CSV)
                    foreach (var mcq in mcqs)
                    {
                        mcq.SubjectUnitID = SubjectUnitID;
                    }

                    _context.MCQQuestions.AddRange(mcqs);
                    _context.SaveChanges();
                }

                TempData["Success"] = "MCQs uploaded successfully!";
            }
            else
            {
                TempData["Error"] = "Please upload a valid CSV file.";
            }

            return RedirectToAction("UploadMCQs");
        }

      


    }
}