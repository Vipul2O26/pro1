using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using pro1.Models;
using Microsoft.AspNetCore.Authorization;


namespace pro1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Dashboard()
    {
        ViewBag.Name = User.Identity.Name;
        ViewBag.Role = User.FindFirst(ClaimTypes.Role)?.Value;
        return View();
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
