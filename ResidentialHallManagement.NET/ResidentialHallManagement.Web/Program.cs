using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Core.Interfaces;
using ResidentialHallManagement.Data;
using ResidentialHallManagement.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// MVC
// ---------------------------------------------------------

builder.Services.AddControllersWithViews();

// ---------------------------------------------------------
// SESSION CONFIGURATION
// ---------------------------------------------------------

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ---------------------------------------------------------
// CONNECTION STRINGS
// ---------------------------------------------------------

var hallConnection = builder.Configuration.GetConnectionString("HallManagementConnection")
    ?? throw new InvalidOperationException("Connection string 'HallManagementConnection' not found.");

var identityConnection = builder.Configuration.GetConnectionString("IdentityConnection")
    ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");

// ---------------------------------------------------------
// DATABASE CONTEXTS (MySQL / XAMPP)
// ---------------------------------------------------------

builder.Services.AddDbContext<HallManagementDbContext>(options =>
    options.UseMySql(
        hallConnection,
        ServerVersion.AutoDetect(hallConnection)
    )
);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        identityConnection,
        ServerVersion.AutoDetect(identityConnection)
    )
);

// ---------------------------------------------------------
// IDENTITY CONFIGURATION
// ---------------------------------------------------------

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ---------------------------------------------------------
// APPLICATION SERVICES (DI)
// ---------------------------------------------------------

builder.Services.AddScoped<IHallService, HallService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IRoomChangeRequestService, RoomChangeRequestService>();
builder.Services.AddScoped<IRoomManagementService, RoomManagementService>();

// ---------------------------------------------------------
// COOKIE CONFIGURATION
// ---------------------------------------------------------

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// ---------------------------------------------------------
// BUILD APP
// ---------------------------------------------------------

var app = builder.Build();

// ---------------------------------------------------------
// SEED ROLES
// ---------------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "HallAdmin", "Student" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed sample BoarderRegistry entries (if not present)
    var db = scope.ServiceProvider.GetRequiredService<HallManagementDbContext>();
    if (!db.BoarderRegistries.Any())
    {
        db.BoarderRegistries.AddRange(new BoarderRegistry { BoarderNo = "B001", Name = "Boarder One", Status = "Available" },
                                     new BoarderRegistry { BoarderNo = "B002", Name = "Boarder Two", Status = "Available" },
                                     new BoarderRegistry { BoarderNo = "B003", Name = "Boarder Three", Status = "Available" });
        await db.SaveChangesAsync();
    }
}

// ---------------------------------------------------------
// MIDDLEWARE PIPELINE
// ---------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Startup DB check for common schema issues (table naming mismatch)
using (var scope2 = app.Services.CreateScope())
{
    var svc = scope2.ServiceProvider;
    try
    {
        var db = svc.GetRequiredService<HallManagementDbContext>();
        try
        {
            if (db.Database.IsRelational())
            {
                db.Database.ExecuteSqlRaw("SELECT 1 FROM rooms LIMIT 1;");
            }
            else
            {
                // Skip checks for non-relational providers (e.g., in-memory for tests)
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("WARNING: Could not find or query 'rooms' table. If your database uses a different table name, check your DbContext mapping. Error: " + ex.Message);
        }
    }
    catch { /* Ignore if context not configured or testing environment */ }
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ---------------------------------------------------------
// ROUTING
// ---------------------------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Expose Program class for integration testing
public partial class Program { }
