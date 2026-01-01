using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResidentialHallManagement.Core.Interfaces;
using ResidentialHallManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using ResidentialHallManagement.Data;

namespace ResidentialHallManagement.Web.Controllers;

[Authorize(Roles = "Admin,HallAdmin")]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly IHallService _hallService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly HallManagementDbContext _dbContext;

    public StudentController(IStudentService studentService, IHallService hallService, UserManager<ApplicationUser> userManager, HallManagementDbContext dbContext)
    {
        _studentService = studentService;
        _hallService = hallService;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    // GET: Student
    public async Task<IActionResult> Index()
    {
        IEnumerable<Student> students;
        if (User.IsInRole("HallAdmin"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();
            if (user.HallId == null) return Forbid();
            students = await _studentService.GetStudentsByHallIdAsync(user.HallId.Value);
        }
        else
        {
            students = await _studentService.GetAllStudentsAsync();
        }
        return View(students);
    }

    // GET: Student/Details/S201201030
    public async Task<IActionResult> Details(string id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        if (User.IsInRole("HallAdmin"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.HallId == null) return Forbid();
            var isAssignedToHall = student.AssignedHalls.Any(ah => ah.HallId == user.HallId && ah.IsCurrent == "YES");
            if (!isAssignedToHall) return Forbid();
        }
        return View(student);
    }

    // GET: Student/Create
    public async Task<IActionResult> Create()
    {
        if (User.IsInRole("HallAdmin"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.HallId != null)
            {
                var hall = await _hallService.GetHallByIdAsync(user.HallId.Value);
                if (hall != null)
                {
                    ViewBag.Halls = new List<Hall> { hall };
                }
                else
                {
                    ViewBag.Halls = await _hallService.GetAllHallsAsync();
                }
            }
            else
            {
                ViewBag.Halls = await _hallService.GetAllHallsAsync();
            }
        }
        else
        {
            ViewBag.Halls = await _hallService.GetAllHallsAsync();
        }
        return View();
    }

    // POST: Student/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (ModelState.IsValid)
        {
            await _studentService.CreateStudentAsync(student);
            TempData["Success"] = "Student created successfully!";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Halls = await _hallService.GetAllHallsAsync();
        return View(student);
    }

    // GET: Student/Edit/S201201030
    public async Task<IActionResult> Edit(string id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        if (User.IsInRole("HallAdmin"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.HallId == null) return Forbid();
            var isAssignedToHall = student.AssignedHalls.Any(ah => ah.HallId == user.HallId && ah.IsCurrent == "YES");
            if (!isAssignedToHall) return Forbid();
        }
        return View(student);
    }

    // POST: Student/Edit/S201201030
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Student student)
    {
        if (id != student.StudentId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _studentService.UpdateStudentAsync(student);
            TempData["Success"] = "Student updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: Student/AssignHall/S201201030
    public async Task<IActionResult> AssignHall(string id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        if (User.IsInRole("HallAdmin"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.HallId != null)
            {
                var hall = await _hallService.GetHallByIdAsync(user.HallId.Value);
                if (hall != null) ViewBag.Halls = new List<Hall> { hall };
                else ViewBag.Halls = await _hallService.GetAllHallsAsync();
            }
            else
            {
                ViewBag.Halls = await _hallService.GetAllHallsAsync();
            }
        }
        else
        {
            ViewBag.Halls = await _hallService.GetAllHallsAsync();
        }
        return View(student);
    }

    // POST: Student/AssignHall
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignHall(string studentId, int hallId, string studentType)
    {
        if (User.IsInRole("HallAdmin"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.HallId == null || user.HallId.Value != hallId)
            {
                return Forbid();
            }
        }
        await _studentService.AssignHallToStudentAsync(studentId, hallId, studentType);
        TempData["Success"] = "Hall assigned successfully!";
        return RedirectToAction(nameof(Details), new { id = studentId });
    }

    // GET: Student/History/S201201030
    public async Task<IActionResult> History(string id)
    {
        var history = await _studentService.GetStudentHallHistoryAsync(id);
        ViewBag.StudentId = id;
        return View(history);
    }

    // GET: Student/Search
    public async Task<IActionResult> Search(string? name, string? id, string? status)
    {
        IEnumerable<Student> students;
        if (User.IsInRole("HallAdmin"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.HallId == null) return Forbid();
            students = await _studentService.GetStudentsByHallIdAsync(user.HallId.Value);
            if (!string.IsNullOrEmpty(name)) students = students.Where(s => s.StudentName.Contains(name));
            if (!string.IsNullOrEmpty(id)) students = students.Where(s => s.StudentId.Contains(id));
            if (!string.IsNullOrEmpty(status)) students = students.Where(s => s.Status == status.ToUpper());
        }
        else
        {
            students = await _studentService.SearchStudentsAsync(name, id, status);
        }
        return View("Index", students);
    }
}
