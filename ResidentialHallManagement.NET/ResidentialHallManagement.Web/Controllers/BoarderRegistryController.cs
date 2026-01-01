using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Data;

namespace ResidentialHallManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class BoarderRegistryController : Controller
{
    private readonly HallManagementDbContext _context;

    public BoarderRegistryController(HallManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var regs = await _context.BoarderRegistries.ToListAsync();
        return View(regs);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest();
        var reg = await _context.BoarderRegistries.FindAsync(id);
        if (reg == null) return NotFound();
        return View(reg);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BoarderRegistry reg)
    {
        // Convert empty strings to null for optional fields
        if (string.IsNullOrWhiteSpace(reg.Name))
            reg.Name = null;
        if (string.IsNullOrWhiteSpace(reg.StudentId))
            reg.StudentId = null;

        // Remove validation errors for optional fields
        ModelState.Remove(nameof(reg.Name));
        ModelState.Remove(nameof(reg.StudentId));

        // Validate required fields
        if (string.IsNullOrWhiteSpace(reg.BoarderNo))
        {
            ModelState.AddModelError(nameof(reg.BoarderNo), "Boarder No is required.");
        }

        // Set default status
        if (string.IsNullOrWhiteSpace(reg.Status))
        {
            reg.Status = "Available";
        }

        if (!ModelState.IsValid) return View(reg);

        try
        {
            // Create a new clean entity to avoid EF tracking issues
            var newRegistry = new BoarderRegistry
            {
                BoarderNo = reg.BoarderNo,
                Name = reg.Name,  // Keep the name even if student doesn't exist
                StudentId = reg.StudentId,  // Will be null if student doesn't exist
                Status = reg.Status
            };

            _context.BoarderRegistries.Add(newRegistry);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Boarder registry entry created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            if (errorMessage.Contains("foreign key constraint"))
            {
                ModelState.AddModelError("", "Database error: The Student ID provided does not exist in the system. The system cannot pre-assign a Student ID that doesn't exist due to database constraints. Please leave the Student ID field empty and only fill in the Name field, or create the student record first.");
            }
            else
            {
                ModelState.AddModelError("", $"Database error: {errorMessage}");
            }
            return View(reg);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id)) return BadRequest();
        var reg = await _context.BoarderRegistries.FindAsync(id);
        if (reg == null) return NotFound();
        return View(reg);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, BoarderRegistry reg)
    {
        if (id != reg.BoarderNo) return BadRequest();

        // Convert empty strings to null for optional fields
        if (string.IsNullOrWhiteSpace(reg.Name))
            reg.Name = null;
        if (string.IsNullOrWhiteSpace(reg.StudentId))
            reg.StudentId = null;

        // Remove validation errors for optional fields
        ModelState.Remove(nameof(reg.Name));
        ModelState.Remove(nameof(reg.StudentId));

        // Check if Student ID exists in database (if provided)
        if (!string.IsNullOrWhiteSpace(reg.StudentId))
        {
            var studentExists = await _context.Students.AnyAsync(s => s.StudentId == reg.StudentId);
            if (!studentExists)
            {
                // Student doesn't exist - clear the StudentId to avoid FK constraint
                TempData["Warning"] = $"Note: Student ID '{reg.StudentId}' does not exist in the system yet. Boarder number '{reg.BoarderNo}' has been updated as 'Available'. The student can register later with this boarder number.";
                reg.StudentId = null;
                reg.Name = null; // Also clear name since student doesn't exist
                reg.Status = "Available"; // Force status to Available
            }
        }

        // Ensure Status has a default value
        if (string.IsNullOrWhiteSpace(reg.Status))
        {
            reg.Status = "Available";
        }

        if (!ModelState.IsValid) return View(reg);

        try
        {
            // Ensure the Student navigation property is null to prevent EF from trying to resolve FK
            reg.Student = null;

            _context.BoarderRegistries.Update(reg);
            await _context.SaveChangesAsync();

            if (TempData["Warning"] == null)
            {
                TempData["Success"] = "Boarder registry entry updated successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            if (errorMessage.Contains("foreign key constraint"))
            {
                ModelState.AddModelError("", "Database error: The Student ID provided does not exist in the system. Please leave the Student ID field empty or provide a valid Student ID from an existing student.");
            }
            else
            {
                ModelState.AddModelError("", $"Database error: {errorMessage}");
            }
            return View(reg);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var reg = await _context.BoarderRegistries.FindAsync(id);
        if (reg == null) return NotFound();
        _context.BoarderRegistries.Remove(reg);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Boarder removed.";
        return RedirectToAction(nameof(Index));
    }
}
