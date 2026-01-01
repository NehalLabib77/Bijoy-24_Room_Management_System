using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ResidentialHallManagement.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class SeatApplicationController : Controller
    {
        private readonly HallManagementDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int SEAT_HOLD_MINUTES = 10; // Time to hold a seat before it expires

        public SeatApplicationController(
            HallManagementDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Main page for browsing and applying for rooms/seats
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found in your profile.";
                return RedirectToAction("Index", "StudentDashboard");
            }

            // Check if student already has an active booking
            var existingBooking = await _context.SeatApplications
                .Include(sa => sa.Seat)
                .ThenInclude(s => s!.Room)
                .ThenInclude(r => r!.Hall)
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId &&
                    (sa.Status == "Approved" || sa.Status == "Pending"));

            if (existingBooking != null)
            {
                ViewBag.ExistingBooking = existingBooking;
                ViewBag.CanApply = false;
            }
            else
            {
                ViewBag.CanApply = true;
            }

            // Get student info for gender-based hall filtering
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                TempData["Error"] = "Student record not found.";
                return RedirectToAction("Index", "StudentDashboard");
            }

            // Get halls matching student's gender
            var halls = await _context.Halls
                .Where(h => h.HallType == student.Gender)
                .OrderBy(h => h.HallName)
                .ToListAsync();

            ViewBag.Student = student;
            ViewBag.Halls = halls;

            return View();
        }

        /// <summary>
        /// Get available rooms in a specific hall
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRooms(int hallId)
        {
            var rooms = await _context.Rooms
                .Where(r => r.HallId == hallId && r.Status == "Available")
                .OrderBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber)
                .Select(r => new
                {
                    r.RoomId,
                    r.HallId,
                    r.RoomNumber,
                    r.RoomName,
                    r.Floor,
                    r.Block,
                    r.Wing,
                    r.RoomCapacity,
                    r.AvailableSlots,
                    r.Status
                })
                .ToListAsync();

            return Json(rooms);
        }

        /// <summary>
        /// Interactive room layout view with seat selection
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> RoomLayout(string roomId, int hallId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found.";
                return RedirectToAction("Index");
            }

            // Check if student already has an active booking
            var existingBooking = await _context.SeatApplications
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId &&
                    (sa.Status == "Approved" || sa.Status == "Pending"));

            if (existingBooking != null)
            {
                TempData["Error"] = "You already have an active booking or pending application.";
                return RedirectToAction("Index");
            }

            var room = await _context.Rooms
                .Include(r => r.Hall)
                .FirstOrDefaultAsync(r => r.RoomId == roomId && r.HallId == hallId);

            if (room == null)
            {
                TempData["Error"] = "Room not found.";
                return RedirectToAction("Index");
            }

            // Get or create seats for this room
            var seats = await GetOrCreateSeatsForRoom(roomId, hallId);

            // Release any expired holds
            await ReleaseExpiredHolds();

            // Get fresh seat data
            seats = await _context.Seats
                .Where(s => s.RoomId == roomId && s.HallId == hallId)
                .OrderBy(s => s.SeatNumber)
                .ToListAsync();

            ViewBag.Room = room;
            ViewBag.Seats = seats;
            ViewBag.StudentId = studentId;

            return View();
        }

        /// <summary>
        /// Get seat status via AJAX for real-time updates
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSeatStatus(string roomId, int hallId)
        {
            await ReleaseExpiredHolds();

            var seats = await _context.Seats
                .Where(s => s.RoomId == roomId && s.HallId == hallId)
                .Select(s => new
                {
                    s.SeatId,
                    s.SeatNumber,
                    s.SeatType,
                    s.Status,
                    s.SeatLabel,
                    s.IsTemporarilyHeld,
                    s.HeldByStudentId,
                    IsAvailable = s.Status == "Available" && !s.IsTemporarilyHeld
                })
                .ToListAsync();

            return Json(seats);
        }

        /// <summary>
        /// Temporarily hold a seat for a student
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoldSeat([FromBody] HoldSeatRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Not authenticated" });

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
                return Json(new { success = false, message = "Student ID not found" });

            // Release any previous holds by this student
            var previousHolds = await _context.Seats
                .Where(s => s.HeldByStudentId == studentId && s.IsTemporarilyHeld)
                .ToListAsync();

            foreach (var hold in previousHolds)
            {
                hold.IsTemporarilyHeld = false;
                hold.HeldByStudentId = null;
                hold.HeldUntil = null;
            }

            // Check if seat is available
            var seat = await _context.Seats.FindAsync(request.SeatId);
            if (seat == null)
                return Json(new { success = false, message = "Seat not found" });

            if (seat.Status != "Available")
                return Json(new { success = false, message = "This seat is already booked" });

            if (seat.IsTemporarilyHeld && seat.HeldByStudentId != studentId)
                return Json(new { success = false, message = "This seat is being held by another student" });

            // Hold the seat
            seat.IsTemporarilyHeld = true;
            seat.HeldByStudentId = studentId;
            seat.HeldUntil = DateTime.UtcNow.AddMinutes(SEAT_HOLD_MINUTES);

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Seat held for {SEAT_HOLD_MINUTES} minutes",
                holdExpiry = seat.HeldUntil
            });
        }

        /// <summary>
        /// Release a seat hold
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReleaseSeat([FromBody] ReleaseSeatRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Not authenticated" });

            var studentId = user.StudentId;

            var seat = await _context.Seats.FindAsync(request.SeatId);
            if (seat == null)
                return Json(new { success = false, message = "Seat not found" });

            if (seat.HeldByStudentId != studentId)
                return Json(new { success = false, message = "You don't have a hold on this seat" });

            seat.IsTemporarilyHeld = false;
            seat.HeldByStudentId = null;
            seat.HeldUntil = null;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Seat released" });
        }

        /// <summary>
        /// Submit seat application
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitApplication([FromBody] SubmitApplicationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "Not authenticated" });

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
                return Json(new { success = false, message = "Student ID not found" });

            // Check for existing active applications
            var existingApp = await _context.SeatApplications
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId &&
                    (sa.Status == "Approved" || sa.Status == "Pending"));

            if (existingApp != null)
                return Json(new { success = false, message = "You already have an active booking or pending application" });

            // Verify seat is held by this student
            var seat = await _context.Seats
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.SeatId == request.SeatId);

            if (seat == null)
                return Json(new { success = false, message = "Seat not found" });

            // Check seat status - only available seats can be booked
            if (seat.Status == SeatStatuses.Booked)
                return Json(new { success = false, message = "This seat is already booked" });

            if (seat.Status == SeatStatuses.Pending || seat.Status == SeatStatuses.Reserved)
                return Json(new { success = false, message = "This seat has a pending application" });

            if (!seat.IsTemporarilyHeld || seat.HeldByStudentId != studentId)
                return Json(new { success = false, message = "Please select and hold a seat first" });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create the application
                var application = new SeatApplication
                {
                    StudentId = studentId,
                    SeatId = seat.SeatId,
                    HallId = seat.HallId,
                    RoomId = seat.RoomId,
                    ApplicationDate = DateTime.UtcNow,
                    Status = "Pending",
                    StudentRemarks = request.Remarks,
                    AcademicYear = DateTime.Now.Year.ToString(),
                    Semester = GetCurrentSemester(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.SeatApplications.Add(application);

                // Update seat status to Pending (waiting for admin approval)
                seat.Status = SeatStatuses.Pending;
                seat.BookedByStudentId = studentId;
                seat.BookedOn = DateTime.UtcNow;
                seat.IsTemporarilyHeld = false;
                seat.HeldByStudentId = null;
                seat.HeldUntil = null;
                seat.UpdatedAt = DateTime.UtcNow;

                // Note: Don't update room available slots until approval

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Application submitted successfully! Awaiting admin approval.",
                    applicationId = application.ApplicationId
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        /// <summary>
        /// View my application status
        /// </summary>
        public async Task<IActionResult> MyApplication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found.";
                return RedirectToAction("Index", "StudentDashboard");
            }

            var applications = await _context.SeatApplications
                .Include(sa => sa.Seat)
                .ThenInclude(s => s!.Room)
                .ThenInclude(r => r!.Hall)
                .Include(sa => sa.Student)
                .Where(sa => sa.StudentId == studentId)
                .OrderByDescending(sa => sa.ApplicationDate)
                .ToListAsync();

            return View(applications);
        }

        /// <summary>
        /// Cancel pending application
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelApplication(int applicationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;

            var application = await _context.SeatApplications
                .Include(sa => sa.Seat)
                .ThenInclude(s => s!.Room)
                .FirstOrDefaultAsync(sa => sa.ApplicationId == applicationId && sa.StudentId == studentId);

            if (application == null)
            {
                TempData["Error"] = "Application not found.";
                return RedirectToAction("MyApplication");
            }

            if (application.Status != "Pending")
            {
                TempData["Error"] = "Only pending applications can be cancelled.";
                return RedirectToAction("MyApplication");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update application status
                application.Status = "Cancelled";
                application.UpdatedAt = DateTime.UtcNow;

                // Release the seat
                if (application.Seat != null)
                {
                    application.Seat.Status = "Available";
                    application.Seat.BookedByStudentId = null;
                    application.Seat.BookedOn = null;
                    application.Seat.UpdatedAt = DateTime.UtcNow;

                    // Update room available slots
                    if (application.Seat.Room != null)
                    {
                        application.Seat.Room.AvailableSlots++;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Application cancelled successfully.";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "An error occurred while cancelling the application.";
            }

            return RedirectToAction("MyApplication");
        }

        #region Helper Methods

        private async Task<List<Seat>> GetOrCreateSeatsForRoom(string roomId, int hallId)
        {
            var existingSeats = await _context.Seats
                .Where(s => s.RoomId == roomId && s.HallId == hallId)
                .ToListAsync();

            if (existingSeats.Any())
                return existingSeats;

            // Create 4 seats for the room with proper seat types
            var seats = new List<Seat>
            {
                new Seat
                {
                    RoomId = roomId,
                    HallId = hallId,
                    SeatNumber = 1,
                    SeatType = SeatTypes.WINDOW_LEFT,
                    SeatLabel = "Window Side - Left",
                    Position = "LEFT",
                    Status = SeatStatuses.Available
                },
                new Seat
                {
                    RoomId = roomId,
                    HallId = hallId,
                    SeatNumber = 2,
                    SeatType = SeatTypes.WINDOW_RIGHT,
                    SeatLabel = "Window Side - Right",
                    Position = "RIGHT",
                    Status = SeatStatuses.Available
                },
                new Seat
                {
                    RoomId = roomId,
                    HallId = hallId,
                    SeatNumber = 3,
                    SeatType = SeatTypes.DOOR_LEFT,
                    SeatLabel = "Door Side - Left",
                    Position = "LEFT",
                    Status = SeatStatuses.Available
                },
                new Seat
                {
                    RoomId = roomId,
                    HallId = hallId,
                    SeatNumber = 4,
                    SeatType = SeatTypes.DOOR_RIGHT,
                    SeatLabel = "Door Side - Right",
                    Position = "RIGHT",
                    Status = SeatStatuses.Available
                }
            };

            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();

            return seats;
        }

        private async Task ReleaseExpiredHolds()
        {
            var expiredHolds = await _context.Seats
                .Where(s => s.IsTemporarilyHeld && s.HeldUntil < DateTime.UtcNow)
                .ToListAsync();

            foreach (var seat in expiredHolds)
            {
                seat.IsTemporarilyHeld = false;
                seat.HeldByStudentId = null;
                seat.HeldUntil = null;
            }

            if (expiredHolds.Any())
                await _context.SaveChangesAsync();
        }

        private string GetCurrentSemester()
        {
            var month = DateTime.Now.Month;
            if (month >= 1 && month <= 6)
                return "Spring";
            return "Fall";
        }

        #endregion
    }

    #region Request Models

    public class HoldSeatRequest
    {
        public int SeatId { get; set; }
    }

    public class ReleaseSeatRequest
    {
        public int SeatId { get; set; }
    }

    public class SubmitApplicationRequest
    {
        public int SeatId { get; set; }
        public string? Remarks { get; set; }
    }

    #endregion
}
