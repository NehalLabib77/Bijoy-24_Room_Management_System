using Microsoft.AspNetCore.Mvc;
using ResidentialHallManagement.Data;
using ResidentialHallManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ResidentialHallManagement.Web.Controllers;

public class AdminAuthController : Controller
{
    private readonly HallManagementDbContext _dbContext;

    public AdminAuthController(HallManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Redirect to unified login
    [HttpGet]
    public IActionResult Login()
    {
        return RedirectToAction("Login", "Account");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}
