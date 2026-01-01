using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace ResidentialHallManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly HallManagementDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(HallManagementDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Index action with role-based redirection
    /// - System Admin → Admin/Dashboard
    /// - Hall Admin → HallAdmin/Dashboard
    /// - Student → StudentDashboard/Index
    /// - Not logged in → Public home page
    /// </summary>
    public async Task<IActionResult> Index()
    {
        // Check session-based admin authentication (for System Admin and Hall Admin)
        var adminRole = HttpContext.Session.GetString("AdminRole");
        var isAdminAuthenticated = HttpContext.Session.GetString("IsAdminAuthenticated") == "true";

        if (isAdminAuthenticated && !string.IsNullOrEmpty(adminRole))
        {
            // Redirect System Admin
            if (adminRole == "SystemAdmin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            // Redirect Hall Admin
            if (adminRole == "HallAdmin")
            {
                return RedirectToAction("Dashboard", "HallAdmin");
            }
        }

        // Check ASP.NET Identity authentication (for Students)
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "StudentDashboard");
            }

            // Fallback for other Identity roles
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (User.IsInRole("HallAdmin"))
            {
                return RedirectToAction("Dashboard", "HallAdmin");
            }
        }

        // Not logged in - show public home page with statistics
        ViewBag.TotalHalls = await _context.Halls.CountAsync();
        ViewBag.TotalStudents = await _context.Students.CountAsync();
        ViewBag.TotalRooms = await _context.Rooms.CountAsync();

        // Get home page images from /assets/home/ folder
        ViewBag.HomeImages = GetHomePageImages();

        return View();
    }

    /// <summary>
    /// Scans the /assets/home/ folder for images and returns their URLs
    /// </summary>
    private List<string> GetHomePageImages()
    {
        var images = new List<string>();
        var homeImagesPath = Path.Combine(_webHostEnvironment.WebRootPath, "assets", "home");

        if (Directory.Exists(homeImagesPath))
        {
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

            var imageFiles = Directory.GetFiles(homeImagesPath)
                .Where(f => supportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .OrderBy(f => f)
                .ToList();

            foreach (var file in imageFiles)
            {
                var fileName = Path.GetFileName(file);
                images.Add($"/assets/home/{fileName}");
            }
        }

        return images;
    }

    /// <summary>
    /// API endpoint to get home page images dynamically (for AJAX calls)
    /// </summary>
    [HttpGet]
    public IActionResult GetHomeImages()
    {
        var images = GetHomePageImages();
        return Json(images);
    }

    /// <summary>
    /// UserDashboard action - redirects to appropriate dashboard based on user role
    /// </summary>
    public IActionResult UserDashboard()
    {
        // Priority 1: Check session-based admin authentication (for System Admin and Hall Admin)
        var adminRole = HttpContext.Session.GetString("AdminRole");
        var isAdminAuthenticated = HttpContext.Session.GetString("IsAdminAuthenticated") == "true";

        if (isAdminAuthenticated && !string.IsNullOrEmpty(adminRole))
        {
            if (adminRole == "SystemAdmin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            if (adminRole == "HallAdmin")
            {
                return RedirectToAction("Dashboard", "HallAdmin");
            }
        }

        // Priority 2: Check ASP.NET Identity authentication (for Students and other roles)
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "StudentDashboard");
            }
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            if (User.IsInRole("HallAdmin"))
            {
                return RedirectToAction("Dashboard", "HallAdmin");
            }
        }

        // Not authenticated - redirect to home
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
