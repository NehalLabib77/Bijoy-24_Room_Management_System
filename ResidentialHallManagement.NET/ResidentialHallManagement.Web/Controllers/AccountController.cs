using Microsoft.AspNetCore.Mvc;
using ResidentialHallManagement.Data;
using Microsoft.AspNetCore.Identity;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ResidentialHallManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly HallManagementDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, HallManagementDbContext dbContext, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var halls = await _dbContext.Halls.ToListAsync();
        ViewBag.Halls = halls;

        // Get faculties from configuration
        var faculties = _configuration.GetSection("SystemConfiguration:Faculties").Get<List<string>>() ?? new List<string> { "Engineering", "Science", "Arts", "Business Studies", "Law", "Medicine" };
        ViewBag.Faculties = faculties;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all required fields correctly.";
            return RedirectToAction("UnifiedAuth");
        }

        // SECURITY: Only allow Student registration through this endpoint
        if (model.UserType != "Student")
        {
            TempData["Error"] = "Invalid registration attempt. Students only.";
            return RedirectToAction("UnifiedAuth");
        }

        // If registering as Student, BoarderNo, StudentId, Faculty and Semester are required
        if (model.UserType == "Student")
        {
            if (string.IsNullOrEmpty(model.BoarderNo))
            {
                TempData["Error"] = "Boarder No is required for student registration.";
                return RedirectToAction("UnifiedAuth");
            }
            if (string.IsNullOrEmpty(model.StudentId))
            {
                TempData["Error"] = "Student ID is required for student registration.";
                return RedirectToAction("UnifiedAuth");
            }
            if (string.IsNullOrEmpty(model.Faculty))
            {
                TempData["Error"] = "Faculty is required for student registration.";
                return RedirectToAction("UnifiedAuth");
            }
            if (!model.Semester.HasValue || model.Semester.Value <= 0)
            {
                TempData["Error"] = "Valid Semester is required for student registration.";
                return RedirectToAction("UnifiedAuth");
            }

            // CRITICAL: Validate that StudentId matches the BoarderNo assignment
            var registry = await _dbContext.BoarderRegistries.FindAsync(model.BoarderNo);
            if (registry == null)
            {
                TempData["Error"] = "Invalid Boarder No.";
                return RedirectToAction("UnifiedAuth");
            }
            if (registry.Status == "Assigned" && registry.StudentId != model.StudentId)
            {
                TempData["Error"] = "This Boarder No is already assigned to another student.";
                return RedirectToAction("UnifiedAuth");
            }
            if (registry.Status == "Available")
            {
                // Pre-assign the StudentId to this BoarderNo for validation
                // This ensures only the correct StudentId can register with this BoarderNo
                if (registry.StudentId != null && registry.StudentId != model.StudentId)
                {
                    TempData["Error"] = $"This Boarder No is pre-assigned to Student ID: {registry.StudentId}. Your Student ID must match.";
                    return RedirectToAction("UnifiedAuth");
                }
            }
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            UserType = model.UserType
        };
        if (!string.IsNullOrEmpty(model.StudentId))
        {
            user.StudentId = model.StudentId;
        }
        if (!string.IsNullOrEmpty(model.FullName))
        {
            user.FullName = model.FullName;
        }
        if (model.HallId.HasValue)
        {
            user.HallId = model.HallId.Value;
        }

        if (model.Password != model.ConfirmPassword)
        {
            TempData["Error"] = "Password and confirmation do not match.";
            return RedirectToAction("UnifiedAuth");
        }

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Add user to the selected role (Student only - security enforced above)
            await _userManager.AddToRoleAsync(user, "Student");

            // If registering as Student and a StudentId was provided, create a Student record if it doesn't exist
            if (model.UserType == "Student")
            {
                // Create or update Student record if StudentId is provided
                if (!string.IsNullOrEmpty(model.StudentId))
                {
                    var existingStudent = await _dbContext.Students.FindAsync(model.StudentId);
                    if (existingStudent == null)
                    {
                        var newStudent = new ResidentialHallManagement.Core.Entities.Student
                        {
                            StudentId = model.StudentId,
                            StudentName = model.FullName ?? model.Username,
                            FatherName = string.Empty,
                            MotherName = string.Empty,
                            Gender = string.Empty,
                            Religion = string.Empty,
                            Mobile = string.Empty,
                            BloodGroup = string.Empty,
                            PermanentAddress = string.Empty,
                            Status = "RUNNING",
                            BoarderNo = model.BoarderNo ?? string.Empty,
                            Faculty = model.Faculty ?? string.Empty,
                            Semester = model.Semester ?? 1
                        };
                        _dbContext.Students.Add(newStudent);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        // If already exists, update BoarderNo, Faculty, Semester
                        existingStudent.BoarderNo = model.BoarderNo ?? existingStudent.BoarderNo;
                        existingStudent.Faculty = model.Faculty ?? existingStudent.Faculty;
                        existingStudent.Semester = model.Semester ?? existingStudent.Semester;
                        _dbContext.Students.Update(existingStudent);
                        await _dbContext.SaveChangesAsync();
                    }

                    // Mark the boarder registry as assigned
                    if (!string.IsNullOrEmpty(model.BoarderNo))
                    {
                        var registry = await _dbContext.BoarderRegistries.FindAsync(model.BoarderNo);
                        if (registry != null)
                        {
                            registry.StudentId = model.StudentId;
                            registry.Status = "Assigned";
                            _dbContext.BoarderRegistries.Update(registry);
                            await _dbContext.SaveChangesAsync();
                        }
                    }
                }
            }

            // Auto sign-in after successful registration
            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["Success"] = "Registration successful! Welcome to the Hall Management System.";
            return RedirectToAction("Index", "StudentDashboard");
        }

        // Registration failed - collect errors
        var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
        TempData["Error"] = $"Registration failed: {errorMessages}";
        return RedirectToAction("UnifiedAuth");
    }

    // GET: Account/UnifiedAuth - Modern sliding login/register page
    [HttpGet]
    public IActionResult UnifiedAuth(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        // Get faculties from configuration
        var faculties = _configuration.GetSection("SystemConfiguration:Faculties").Get<List<string>>() ?? new List<string> { "Engineering", "Science", "Arts", "Business Studies", "Law", "Medicine" };
        ViewBag.Faculties = faculties;

        return View();
    }

    // GET: Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Redirect to unified auth page
        return RedirectToAction("UnifiedAuth", new { returnUrl });
    }

    // POST: Account/Login - Unified login for all user types
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            // Validate UserType is provided
            if (string.IsNullOrEmpty(model.UserType))
            {
                TempData["Error"] = "User type is required. Please select a role tab.";
                return RedirectToAction("UnifiedAuth");
            }

            // SYSTEM ADMIN LOGIN
            if (model.UserType == "Admin")
            {
                var adminUser = await _dbContext.AdminUsers.FirstOrDefaultAsync(a =>
                    a.Username == model.Username && a.Role == "SystemAdmin");

                if (adminUser != null && adminUser.IsActive && adminUser.Password == model.Password)
                {
                    // Create a temporary ApplicationUser for authentication purposes
                    var appUser = new ApplicationUser
                    {
                        UserName = adminUser.Username,
                        Email = adminUser.Username + "@admin.local",
                        UserType = "Admin",
                        FullName = adminUser.Username
                    };

                    // Sign in the user with Identity
                    await _signInManager.SignInAsync(appUser, isPersistent: model.RememberMe);

                    // Also set session variables for backward compatibility
                    HttpContext.Session.SetString("AdminUsername", adminUser.Username);
                    HttpContext.Session.SetString("AdminRole", adminUser.Role);
                    HttpContext.Session.SetString("AdminId", adminUser.AdminUserId.ToString());
                    HttpContext.Session.SetString("IsAdminAuthenticated", "true");
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    TempData["Error"] = "Invalid System Admin credentials.";
                    return RedirectToAction("UnifiedAuth");
                }
            }

            // HALL ADMIN LOGIN
            if (model.UserType == "HallAdmin")
            {
                var hallAdmin = await _dbContext.AdminUsers.FirstOrDefaultAsync(a =>
                    a.Username == model.Username && a.Role == "HallAdmin");

                if (hallAdmin != null && hallAdmin.IsActive && hallAdmin.Password == model.Password)
                {
                    // Create a temporary ApplicationUser for authentication purposes
                    var appUser = new ApplicationUser
                    {
                        UserName = hallAdmin.Username,
                        Email = hallAdmin.Username + "@admin.local",
                        UserType = "HallAdmin",
                        FullName = hallAdmin.Username
                    };

                    // Sign in the user with Identity
                    await _signInManager.SignInAsync(appUser, isPersistent: model.RememberMe);

                    // Also set session variables for backward compatibility
                    HttpContext.Session.SetString("AdminUsername", hallAdmin.Username);
                    HttpContext.Session.SetString("AdminRole", hallAdmin.Role);
                    HttpContext.Session.SetString("AdminId", hallAdmin.AdminUserId.ToString());
                    HttpContext.Session.SetString("IsAdminAuthenticated", "true");
                    return RedirectToAction("Dashboard", "HallAdmin");
                }
                else
                {
                    TempData["Error"] = "Invalid Hall Admin credentials.";
                    return RedirectToAction("UnifiedAuth");
                }
            }

            // STUDENT LOGIN
            if (model.UserType == "Student")
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null && user.UserType == "Student")
                    {
                        return RedirectToAction("Index", "StudentDashboard");
                    }
                    else
                    {
                        // User exists but is not a student
                        await _signInManager.SignOutAsync();
                        TempData["Error"] = "Invalid Student credentials.";
                        return RedirectToAction("UnifiedAuth");
                    }
                }
                else
                {
                    TempData["Error"] = "Invalid Student credentials.";
                    return RedirectToAction("UnifiedAuth");
                }
            }

            TempData["Error"] = "Invalid login attempt.";
            return RedirectToAction("UnifiedAuth");
        }

        return RedirectToAction("UnifiedAuth");
    }

    // POST: Account/Logout - Unified logout for all user types
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Clear Identity authentication
        await _signInManager.SignOutAsync();

        // Clear session (for admin users)
        HttpContext.Session.Clear();

        return RedirectToAction("Login", "Account");
    }

    // GET: Account/AccessDenied
    public IActionResult AccessDenied()
    {
        return View();
    }

    // GET: Account/RegisterHallAdmin
    [HttpGet]
    public IActionResult RegisterHallAdmin()
    {
        return View();
    }

    // POST: Account/RegisterHallAdmin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterHallAdmin(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all required fields correctly.";
            return RedirectToAction("UnifiedAuth");
        }

        // SECURITY: Only allow HallAdmin registration through this endpoint
        if (model.UserType != "HallAdmin")
        {
            TempData["Error"] = "Invalid registration attempt. Hall Admins only.";
            return RedirectToAction("UnifiedAuth");
        }

        // Validate registration token
        if (string.IsNullOrEmpty(model.RegistrationToken))
        {
            TempData["Error"] = "Registration token is required.";
            return RedirectToAction("UnifiedAuth");
        }

        // Find hall admin by registration token
        var hallAdmin = await _dbContext.HallAdmins
            .Include(h => h.Hall)
            .FirstOrDefaultAsync(h => h.RegistrationToken == model.RegistrationToken && !h.IsRegistered);

        if (hallAdmin == null)
        {
            TempData["Error"] = "Invalid or already used registration token.";
            return RedirectToAction("UnifiedAuth");
        }

        // Get the associated admin user
        var adminUser = await _dbContext.AdminUsers
            .FirstOrDefaultAsync(a => a.Username == hallAdmin.UserId);

        if (adminUser == null)
        {
            TempData["Error"] = "Associated admin user not found.";
            return RedirectToAction("UnifiedAuth");
        }

        // Validate password
        if (model.Password != model.ConfirmPassword)
        {
            TempData["Error"] = "Password and confirmation do not match.";
            return RedirectToAction("UnifiedAuth");
        }

        // Update admin user password
        adminUser.Password = model.Password;
        _dbContext.AdminUsers.Update(adminUser);

        // Update hall admin with email and mark as registered
        hallAdmin.Email = model.Email;
        hallAdmin.IsRegistered = true;
        hallAdmin.Status = "Active";
        _dbContext.HallAdmins.Update(hallAdmin);

        await _dbContext.SaveChangesAsync();

        TempData["Success"] = "Hall Admin registration completed successfully! You can now login.";
        return RedirectToAction("UnifiedAuth");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
}
