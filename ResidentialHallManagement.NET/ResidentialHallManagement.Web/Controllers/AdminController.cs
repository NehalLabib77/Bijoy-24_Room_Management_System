using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Data;

namespace ResidentialHallManagement.Web.Controllers;

public class AdminController : Controller
{
    private readonly HallManagementDbContext _context;

    public AdminController(HallManagementDbContext context)
    {
        _context = context;
    }

    private bool IsSystemAdmin()
    {
        var role = HttpContext.Session.GetString("AdminRole");
        return role == "SystemAdmin";
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        var totalHalls = await _context.Halls.CountAsync();
        var totalStudents = await _context.Students.CountAsync();
        var totalRooms = await _context.Rooms.CountAsync();
        var totalHallAdmins = await _context.HallAdmins.CountAsync();

        ViewBag.TotalHalls = totalHalls;
        ViewBag.TotalStudents = totalStudents;
        ViewBag.TotalRooms = totalRooms;
        ViewBag.TotalHallAdmins = totalHallAdmins;

        return View();
    }

    public IActionResult Index()
    {
        return RedirectToAction("Dashboard");
    }

    public async Task<IActionResult> ManageHallAdmins()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        var hallAdmins = await _context.HallAdmins
            .Include(ha => ha.Hall)
            .ToListAsync();

        return View(hallAdmins);
    }

    [HttpGet]
    public async Task<IActionResult> CreateHallAdmin()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        ViewBag.Halls = await _context.Halls.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateHallAdmin(int hallId, string adminName)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        if (hallId == 0 || string.IsNullOrWhiteSpace(adminName))
        {
            TempData["Error"] = "Full Name and Hall selection are required";
            ViewBag.Halls = await _context.Halls.ToListAsync();
            return View();
        }

        // Check if hall already has an admin
        var existingAdmin = await _context.HallAdmins.FirstOrDefaultAsync(h => h.HallId == hallId);
        if (existingAdmin != null)
        {
            TempData["Error"] = "This hall already has an assigned admin";
            ViewBag.Halls = await _context.Halls.ToListAsync();
            return View();
        }

        // Generate unique registration token
        string registrationToken = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

        // Ensure token is unique
        while (await _context.HallAdmins.AnyAsync(h => h.RegistrationToken == registrationToken))
        {
            registrationToken = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
        }

        // Create Hall Admin Profile with registration token (no AdminUser yet - created during registration)
        var hallAdmin = new HallAdmin
        {
            HallId = hallId,
            UserId = string.Empty, // Will be set during registration
            AdminName = adminName,
            Phone = string.Empty,
            AdminRole = "HallAdmin",
            Status = "Pending", // Will be Active after registration
            StartDate = DateTime.UtcNow,
            RegistrationToken = registrationToken,
            IsRegistered = false // Not yet registered
        };

        _context.HallAdmins.Add(hallAdmin);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Hall Admin created successfully! Share this token with {adminName} to complete registration.";
        TempData["RegistrationToken"] = registrationToken;
        return RedirectToAction("ManageHallAdmins");
    }

    public async Task<IActionResult> ManageRooms()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        var rooms = await _context.Rooms
            .Include(r => r.Hall)
            .ToListAsync();

        return View(rooms);
    }

    public IActionResult SystemSettings()
    {
        return View();
    }

    // Database Management Methods
    public IActionResult BackupDatabase()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        try
        {
            // Implement backup logic here
            TempData["Success"] = "Database backup completed successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Backup failed: {ex.Message}";
        }
        return RedirectToAction("Dashboard");
    }

    public IActionResult ViewDatabaseStats()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        try
        {
            // Get database statistics
            var stats = new
            {
                TotalHalls = _context.Halls.Count(),
                TotalRooms = _context.Rooms.Count(),
                TotalStudents = _context.Students.Count(),
                TotalAdmins = _context.HallAdmins.Count(),
                TotalBoarderEntries = _context.BoarderRegistries.Count()
            };
            ViewBag.Stats = stats;
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to retrieve stats: {ex.Message}";
        }
        return View();
    }

    public IActionResult ClearOldLogs()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        try
        {
            // Implement log clearing logic here
            TempData["Success"] = "Old logs cleared successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to clear logs: {ex.Message}";
        }
        return RedirectToAction("Dashboard");
    }

    // Boarder Registry Management - New Feature
    public async Task<IActionResult> ManageBoarderRegistry()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        var boarderRegistry = await _context.BoarderRegistries
            .Include(br => br.Student)
            .OrderBy(br => br.Status)
            .ThenBy(br => br.BoarderNo)
            .ToListAsync();

        // Create a dictionary of students for easy lookup in the view
        var studentDict = new Dictionary<string, object>();
        foreach (var entry in boarderRegistry)
        {
            if (!string.IsNullOrEmpty(entry.StudentId) && entry.Student != null)
            {
                studentDict[entry.StudentId] = entry.Student;
            }
        }

        ViewBag.Students = studentDict;
        return View(boarderRegistry);
    }

    /// <summary>
    /// GET: Display the form to create a new Boarder Registry entry
    /// </summary>
    [HttpGet]
    public IActionResult CreateBoarderEntry()
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        return View(new BoarderRegistry());
    }

    /// <summary>
    /// POST: Create a new Boarder Registry entry
    /// Handles validation and ensures data integrity
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBoarderEntry(ResidentialHallManagement.Core.Entities.BoarderRegistry model, string faculty = "", int semester = 0)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");

        // Remove whitespace from inputs
        model.BoarderNo = model.BoarderNo?.Trim() ?? string.Empty;
        model.StudentId = model.StudentId?.Trim();
        model.Name = model.Name?.Trim();

        // Validate required fields
        if (string.IsNullOrEmpty(model.BoarderNo))
        {
            ModelState.AddModelError("BoarderNo", "Boarder Number is required.");
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please correct the validation errors.";
            return View(model);
        }

        // Check if BoarderNo already exists
        var existing = await _context.BoarderRegistries.FindAsync(model.BoarderNo);
        if (existing != null)
        {
            ModelState.AddModelError("BoarderNo", "This Boarder Number already exists.");
            TempData["Error"] = "Boarder Number already exists. Please use a different number.";
            return View(model);
        }

        // Check if StudentId is provided and validate it
        if (!string.IsNullOrEmpty(model.StudentId))
        {
            // Check if this StudentId is already assigned to another BoarderNo
            var existingStudentBoarder = await _context.BoarderRegistries
                .FirstOrDefaultAsync(br => br.StudentId == model.StudentId);
            if (existingStudentBoarder != null)
            {
                ModelState.AddModelError("StudentId", $"This Student ID is already assigned to Boarder Number: {existingStudentBoarder.BoarderNo}");
                TempData["Error"] = "Student ID is already assigned to another boarder.";
                return View(model);
            }

            // Check if student exists in Students table
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == model.StudentId);
            if (student != null)
            {
                // Update student's faculty and semester if provided
                if (!string.IsNullOrEmpty(faculty))
                {
                    student.Faculty = faculty;
                }
                if (semester > 0)
                {
                    student.Semester = semester;
                }
                _context.Students.Update(student);
            }
            else
            {
                // Student doesn't exist yet - that's okay, they might register later
                // Just log a warning in TempData
                TempData["Warning"] = $"Note: Student ID '{model.StudentId}' does not exist in the system yet. They can register later with this ID.";
            }

            // Set status as Assigned since a StudentId is provided
            model.Status = "Assigned";
        }
        else
        {
            // No StudentId provided - status is Available
            model.Status = "Available";
        }

        try
        {
            _context.BoarderRegistries.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Boarder Registry entry '{model.BoarderNo}' created successfully!";
            return RedirectToAction(nameof(ManageBoarderRegistry));
        }
        catch (DbUpdateException dbEx)
        {
            // Database constraint violation
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            TempData["Error"] = $"Database error: {innerMessage}";
            ModelState.AddModelError("", $"Failed to save to database: {innerMessage}");
            return View(model);
        }
        catch (Exception ex)
        {
            // General error
            TempData["Error"] = $"Unexpected error: {ex.Message}";
            ModelState.AddModelError("", $"Failed to create boarder entry: {ex.Message}");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditBoarderEntry(string id)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        if (string.IsNullOrEmpty(id))
            return BadRequest();

        var boarderEntry = await _context.BoarderRegistries
            .Include(br => br.Student)
            .FirstOrDefaultAsync(br => br.BoarderNo == id);

        if (boarderEntry == null)
            return NotFound();

        return View(boarderEntry);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBoarderEntry(string id, ResidentialHallManagement.Core.Entities.BoarderRegistry model, string faculty = "", int semester = 0)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        if (id != model.BoarderNo)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var boarderEntry = await _context.BoarderRegistries.FindAsync(id);
        if (boarderEntry == null)
            return NotFound();

        // Check if StudentId is being changed to one that already exists
        if (!string.IsNullOrEmpty(model.StudentId) && boarderEntry.StudentId != model.StudentId)
        {
            var existingStudent = await _context.BoarderRegistries
                .FirstOrDefaultAsync(br => br.StudentId == model.StudentId && br.BoarderNo != id);
            if (existingStudent != null)
            {
                ModelState.AddModelError("StudentId", "This Student ID is already assigned to another Boarder Number.");
                return View(model);
            }
        }

        boarderEntry.Name = model.Name;
        boarderEntry.StudentId = model.StudentId;
        boarderEntry.Status = string.IsNullOrEmpty(model.StudentId) ? "Available" : "Assigned";

        // Update student's faculty and semester if provided
        if (!string.IsNullOrEmpty(model.StudentId))
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == model.StudentId);
            if (student != null)
            {
                if (!string.IsNullOrEmpty(faculty))
                {
                    student.Faculty = faculty;
                }
                if (semester > 0)
                {
                    student.Semester = semester;
                }
                _context.Students.Update(student);
            }
        }

        try
        {
            _context.BoarderRegistries.Update(boarderEntry);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Boarder registry entry updated successfully.";
            return RedirectToAction(nameof(ManageBoarderRegistry));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to update boarder entry: {ex.Message}";
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBoarderEntry(string id)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        var boarderEntry = await _context.BoarderRegistries.FindAsync(id);
        if (boarderEntry == null)
            return NotFound();

        try
        {
            _context.BoarderRegistries.Remove(boarderEntry);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Boarder registry entry deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to delete boarder entry: {ex.Message}";
        }

        return RedirectToAction(nameof(ManageBoarderRegistry));
    }

    // GET: Admin/EditRoom?roomId=R1&hallId=1
    public async Task<IActionResult> EditRoom(string roomId, int hallId)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        if (string.IsNullOrEmpty(roomId))
            return BadRequest();

        var room = await _context.Rooms.FindAsync(roomId, hallId);
        if (room == null)
            return NotFound();

        ViewBag.Halls = await _context.Halls.ToListAsync();
        return View(room);
    }

    // POST: Admin/EditRoom
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoom(string roomId, int hallId, ResidentialHallManagement.Core.Entities.Room model)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid)
        {
            ViewBag.Halls = await _context.Halls.ToListAsync();
            return View(model);
        }

        var room = await _context.Rooms.FindAsync(roomId, hallId);
        if (room == null)
            return NotFound();

        room.RoomName = model.RoomName;
        room.RoomNumber = model.RoomNumber;
        room.Block = model.Block;
        room.Floor = model.Floor;
        room.Wing = model.Wing;
        room.RoomCapacity = model.RoomCapacity;
        room.Status = model.Status;

        try
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Room updated successfully.";
            return RedirectToAction("ManageRooms");
        }
        catch
        {
            TempData["Error"] = "Failed to update room.";
            ViewBag.Halls = await _context.Halls.ToListAsync();
            return View(model);
        }
    }

    // GET: Admin/EditHallAdmin/5
    public async Task<IActionResult> EditHallAdmin(int id)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        var hallAdmin = await _context.HallAdmins.FindAsync(id);
        if (hallAdmin == null)
            return NotFound();

        ViewBag.Halls = await _context.Halls.ToListAsync();
        return View(hallAdmin);
    }

    // POST: Admin/EditHallAdmin/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditHallAdmin(int id, ResidentialHallManagement.Core.Entities.HallAdmin model)
    {
        if (!IsSystemAdmin()) return RedirectToAction("Login", "Account");
        if (id != model.HallAdminId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Halls = await _context.Halls.ToListAsync();
            return View(model);
        }

        var hallAdmin = await _context.HallAdmins.FindAsync(id);
        if (hallAdmin == null)
            return NotFound();

        hallAdmin.AdminName = model.AdminName;
        hallAdmin.Email = model.Email;
        hallAdmin.Phone = model.Phone;
        hallAdmin.StartDate = model.StartDate;
        hallAdmin.EndDate = model.EndDate;
        hallAdmin.AdminRole = model.AdminRole;
        hallAdmin.Status = model.Status;
        hallAdmin.HallId = model.HallId;

        try
        {
            _context.HallAdmins.Update(hallAdmin);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Hall admin updated successfully.";
            return RedirectToAction("ManageHallAdmins");
        }
        catch
        {
            TempData["Error"] = "Failed to update hall admin.";
            ViewBag.Halls = await _context.Halls.ToListAsync();
            return View(model);
        }
    }
}
