using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Core.Interfaces;
using ResidentialHallManagement.Data;

namespace ResidentialHallManagement.Web.Controllers;

public class HallAdminController : Controller
{
    private readonly HallManagementDbContext _context;
    private readonly IRoomManagementService _roomManagementService;
    private readonly UserManager<ApplicationUser> _userManager;

    public HallAdminController(
        HallManagementDbContext context,
        IRoomManagementService roomManagementService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _roomManagementService = roomManagementService;
        _userManager = userManager;
    }

    /// <summary>
    /// Check if current user is authenticated as Hall Admin
    /// </summary>
    private bool IsHallAdmin()
    {
        var role = HttpContext.Session.GetString("AdminRole");
        return role == "HallAdmin";
    }

    /// <summary>
    /// Helper method to get current Hall Admin's username from session
    /// </summary>
    private string? GetHallAdminUsername()
    {
        if (!IsHallAdmin()) return null;
        return HttpContext.Session.GetString("AdminUsername");
    }

    /// <summary>
    /// Helper method to get current Hall Admin's HallId from database
    /// </summary>
    private async Task<int?> GetHallAdminHallIdAsync()
    {
        var adminUsername = GetHallAdminUsername();
        if (string.IsNullOrEmpty(adminUsername))
            return null;

        var hallAdmin = await _context.HallAdmins
            .FirstOrDefaultAsync(ha => ha.UserId == adminUsername);

        return hallAdmin?.HallId;
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var adminUsername = HttpContext.Session.GetString("AdminUsername");
        if (string.IsNullOrEmpty(adminUsername))
            return RedirectToAction("Login", "Account");

        // Get hall admin info from admin_users table
        var adminUser = await _context.AdminUsers.FirstOrDefaultAsync(a => a.Username == adminUsername);
        if (adminUser == null)
            return RedirectToAction("Login", "Account");

        // Get associated hall admin profile
        var hallAdmin = await _context.HallAdmins
            .Include(ha => ha.Hall)
            .FirstOrDefaultAsync(ha => ha.UserId == adminUsername);

        if (hallAdmin == null)
            return Unauthorized();

        ViewBag.HallAdmin = hallAdmin;
        ViewBag.Hall = hallAdmin.Hall;

        // Get statistics for this hall
        var hallId = hallAdmin.HallId;

        // Get all room assignments for this hall with student data
        var roomAssignments = await _context.RoomAssignments
            .Include(ra => ra.Student)
            .Where(ra => ra.HallId == hallId && ra.Status == "Active")
            .ToListAsync();

        // Get pending applications for this hall
        var pendingApplications = await _context.RoomApplicationRequests
            .Include(rar => rar.Student)
            .Where(rar => rar.HallId == hallId && rar.Status == "Pending")
            .ToListAsync();

        ViewBag.RoomAssignments = roomAssignments;
        ViewBag.PendingApplications = pendingApplications;

        // Legacy stats (kept for backward compatibility if needed elsewhere)
        ViewBag.TotalRooms = await _context.Rooms.Where(r => r.HallId == hallId).CountAsync();
        ViewBag.AssignedRooms = roomAssignments.Select(ra => ra.RoomIdentity).Distinct().Count();
        ViewBag.TotalStudents = roomAssignments.Count;

        return View();
    }

    // Room Application Management
    public async Task<IActionResult> EditRooms()
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        var hall = await _context.Halls.FindAsync(hallId.Value);
        var rooms = await _context.Rooms.Where(r => r.HallId == hallId.Value).ToListAsync();

        ViewBag.Hall = hall;
        return View(rooms);
    }

    [HttpGet]
    public async Task<IActionResult> CreateRoom()
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        var hall = await _context.Halls.FindAsync(hallId.Value);
        ViewBag.Hall = hall;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoom(Room room)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        // Ensure the room is always created for the logged-in hall admin's hall
        room.HallId = hallId.Value;

        if (string.IsNullOrWhiteSpace(room.RoomId))
        {
            ModelState.AddModelError("RoomId", "Room ID is required.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Hall = await _context.Halls.FindAsync(hallId.Value);
            return View(room);
        }

        try
        {
            // Set available slots equal to capacity
            room.AvailableSlots = room.RoomCapacity;
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Room {room.RoomNumber ?? room.RoomId} created successfully.";
            return RedirectToAction("EditRooms");
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Failed to create room: {ex.Message}";
            ViewBag.Hall = await _context.Halls.FindAsync(hallId.Value);
            return View(room);
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRoom(string roomId, int hallId, int capacity, string? roomName, int? floor, string status, string? wing)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var adminHallId = await GetHallAdminHallIdAsync();
        if (adminHallId == null || adminHallId.Value != hallId)
            return Unauthorized();

        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == roomId && r.HallId == hallId);
        if (room == null)
        {
            TempData["Error"] = "Room not found.";
            return RedirectToAction("EditRooms");
        }

        var oldCapacity = room.RoomCapacity;
        var oldAvailable = room.AvailableSlots;
        room.RoomCapacity = capacity;
        room.RoomName = roomName;
        room.Floor = floor;
        room.Status = status;
        room.Wing = wing;

        // Adjust available slots based on capacity difference
        if (capacity > oldCapacity)
        {
            room.AvailableSlots = oldAvailable + (capacity - oldCapacity);
        }
        else if (capacity < oldCapacity)
        {
            // If new capacity is less than previous, ensure available slots don't go negative
            var diff = oldCapacity - capacity;
            room.AvailableSlots = Math.Max(0, oldAvailable - diff);
        }

        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Room {roomId} updated successfully.";
        return RedirectToAction("EditRooms");
    }

    public async Task<IActionResult> AssignStudent()
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        var hall = await _context.Halls.FindAsync(hallId.Value);
        var availableRooms = await _roomManagementService.GetAvailableRoomsAsync(hallId.Value);

        ViewBag.Hall = hall;
        ViewBag.AvailableRooms = availableRooms;

        return View();
    }

    public async Task<IActionResult> ReviewApplication(int id)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var application = await _roomManagementService.GetApplicationAsync(id);
        if (application == null)
            return NotFound();

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null || hallId.Value != application.HallId)
            return Unauthorized();

        return View(application);
    }

    public async Task<IActionResult> RoomApplications()
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        var applications = await _roomManagementService.GetPendingApplicationsAsync(hallId.Value);

        return View(applications);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveApplication(int applicationId)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var adminUsername = GetHallAdminUsername();
        if (string.IsNullOrEmpty(adminUsername))
            return Unauthorized();

        var result = await _roomManagementService.ApproveApplicationAsync(applicationId, adminUsername, "Approved by Hall Admin");
        if (result)
            TempData["Success"] = "Application approved successfully.";
        else
            TempData["Error"] = "Failed to approve application.";

        return RedirectToAction("RoomApplications");
    }

    [HttpPost]
    public async Task<IActionResult> RejectApplication(int applicationId, string remarks)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var adminUsername = GetHallAdminUsername();
        if (string.IsNullOrEmpty(adminUsername))
            return Unauthorized();

        var result = await _roomManagementService.RejectApplicationAsync(applicationId, adminUsername, remarks ?? "Rejected by Hall Admin");
        if (result)
            TempData["Success"] = "Application rejected successfully.";
        else
            TempData["Error"] = "Failed to reject application.";

        return RedirectToAction("RoomApplications");
    }

    [HttpPost]
    public async Task<IActionResult> AssignFromApplication(int applicationId, string bedNumber)
    {
        var result = await _roomManagementService.AssignFromApplicationAsync(applicationId, bedNumber);
        if (result)
            TempData["Success"] = "Room assigned successfully.";
        else
            TempData["Error"] = "Failed to assign room.";

        return RedirectToAction("RoomApplications");
    }

    // Room Management
    // New: ManageRoom - View room details with member information
    public async Task<IActionResult> ManageRoom(string roomId, int hallId)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var adminHallId = await GetHallAdminHallIdAsync();
        if (adminHallId == null || adminHallId.Value != hallId)
            return Unauthorized();

        var room = await _context.Rooms
            .Include(r => r.Hall)
            .FirstOrDefaultAsync(r => r.RoomId == roomId && r.HallId == hallId);

        if (room == null)
            return NotFound();

        // Get room assignments with student details
        var roomAssignments = await _context.RoomAssignments
            .Include(ra => ra.Student)
            .Where(ra => ra.RoomIdentity.StartsWith(room.RoomNumber + "/") && ra.HallId == hallId && ra.Status == "Active" && ra.Student != null)
            .ToListAsync();

        // Get just the students for backward compatibility
        var roomMembers = roomAssignments.Select(ra => ra.Student!).ToList();

        ViewBag.RoomMembers = roomMembers;
        ViewBag.RoomAssignments = roomAssignments;

        return View(room);
    }

    public async Task<IActionResult> ManageRoomAssignments()
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        var hall = await _context.Halls.FindAsync(hallId.Value);
        var assignments = await _roomManagementService.GetHallRoomAssignmentsAsync(hallId.Value);
        var availableRooms = await _roomManagementService.GetAvailableRoomsAsync(hallId.Value);

        ViewBag.Hall = hall;
        ViewBag.AvailableRooms = availableRooms;

        return View(assignments);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRoomAssignment(int assignmentId, string? roomId, int? hallId)
    {
        var result = await _roomManagementService.RemoveRoomAssignmentAsync(assignmentId);
        if (result)
            TempData["Success"] = "Room assignment removed successfully.";
        else
            TempData["Error"] = "Failed to remove room assignment.";

        // If roomId and hallId are provided, redirect back to ManageRoom
        if (!string.IsNullOrEmpty(roomId) && hallId.HasValue)
            return RedirectToAction("ManageRoom", new { roomId, hallId });

        return RedirectToAction("ManageRoomAssignments");
    }

    [HttpPost]
    public async Task<IActionResult> ManuallyAssignRoom(string studentId, string roomIdentity, string? bedNumber)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        // Validate input
        if (string.IsNullOrWhiteSpace(studentId))
        {
            TempData["Error"] = "Student ID is required.";
            return RedirectToAction("AssignStudent");
        }

        if (string.IsNullOrWhiteSpace(roomIdentity))
        {
            TempData["Error"] = "Room selection is required.";
            return RedirectToAction("AssignStudent");
        }

        // Check if student exists
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
        {
            TempData["Error"] = $"Student with ID '{studentId}' not found.";
            return RedirectToAction("AssignStudent");
        }

        // Check if student already has an active room assignment
        var existingAssignment = await _context.RoomAssignments
            .FirstOrDefaultAsync(ra => ra.StudentId == studentId && ra.Status == "Active");
        if (existingAssignment != null)
        {
            TempData["Error"] = $"Student '{studentId}' is already assigned to room {existingAssignment.RoomIdentity}. Please remove the existing assignment first.";
            return RedirectToAction("AssignStudent");
        }

        // Check if room is available
        if (!await _roomManagementService.IsRoomAvailableAsync(hallId.Value, roomIdentity))
        {
            TempData["Error"] = "Selected room is not available or fully occupied.";
            return RedirectToAction("AssignStudent");
        }

        try
        {
            var assignment = await _roomManagementService.AssignRoomAsync(studentId, hallId.Value, roomIdentity, bedNumber, "Manually assigned by Hall Admin");
            if (assignment != null)
            {
                TempData["Success"] = $"Room {roomIdentity} successfully assigned to student {studentId}.";
            }
            else
            {
                TempData["Error"] = "Failed to assign room. The room may be full or unavailable.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error assigning room: {ex.Message}";
        }

        return RedirectToAction("ManageRoomAssignments");
    }

    // Check room availability
    [HttpGet]
    public async Task<IActionResult> CheckRoomAvailability(int hallId, string roomIdentity)
    {
        var isAvailable = await _roomManagementService.IsRoomAvailableAsync(hallId, roomIdentity);
        var occupancy = await _roomManagementService.GetRoomOccupancyAsync(hallId, roomIdentity);

        return Json(new { available = isAvailable, occupancy = occupancy });
    }

    // ==================== SEAT APPLICATIONS MANAGEMENT ====================

    /// <summary>
    /// View all pending seat applications for the hall
    /// </summary>
    public async Task<IActionResult> SeatApplications()
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        // Get hall info
        var hall = await _context.Halls.FindAsync(hallId.Value);
        ViewBag.Hall = hall;

        // Get all seat applications for this hall with related data
        var applications = await _context.SeatApplications
            .Include(sa => sa.Seat)
                .ThenInclude(s => s!.Room)
            .Include(sa => sa.Student)
            .Where(sa => sa.HallId == hallId.Value)
            .OrderByDescending(sa => sa.ApplicationDate)
            .ToListAsync();

        // Get statistics
        ViewBag.PendingCount = applications.Count(a => a.Status == "Pending");
        ViewBag.ApprovedCount = applications.Count(a => a.Status == "Approved");
        ViewBag.RejectedCount = applications.Count(a => a.Status == "Rejected");
        ViewBag.TotalSeats = await _context.Seats.CountAsync(s => s.HallId == hallId.Value);
        ViewBag.BookedSeats = await _context.Seats.CountAsync(s => s.HallId == hallId.Value && s.Status == SeatStatuses.Booked);

        return View(applications);
    }

    /// <summary>
    /// Approve a seat application and lock the seat
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ApproveSeatApplication(int applicationId)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var adminUsername = GetHallAdminUsername();
        if (string.IsNullOrEmpty(adminUsername))
            return Unauthorized();

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        // Get the application with seat info
        var application = await _context.SeatApplications
            .Include(sa => sa.Seat)
                .ThenInclude(s => s!.Room)
            .Include(sa => sa.Student)
            .FirstOrDefaultAsync(sa => sa.ApplicationId == applicationId);

        if (application == null)
        {
            TempData["Error"] = "Application not found.";
            return RedirectToAction("SeatApplications");
        }

        // Verify it's for the admin's hall
        if (application.HallId != hallId.Value)
        {
            TempData["Error"] = "Unauthorized: This application is not for your hall.";
            return RedirectToAction("SeatApplications");
        }

        // Check if application is still pending
        if (application.Status != "Pending")
        {
            TempData["Error"] = $"This application has already been {application.Status.ToLower()}.";
            return RedirectToAction("SeatApplications");
        }

        // Get the seat and verify it's available
        var seat = application.Seat;
        if (seat == null)
        {
            TempData["Error"] = "Seat not found for this application.";
            return RedirectToAction("SeatApplications");
        }

        // Check if seat is already booked by someone else
        if (seat.Status == SeatStatuses.Booked && seat.BookedByStudentId != application.StudentId)
        {
            TempData["Error"] = "This seat has already been booked by another student.";
            // Auto-reject this application
            application.Status = "Rejected";
            application.AdminRemarks = "Seat was booked by another student.";
            application.ProcessedDate = DateTime.UtcNow;
            application.ProcessedBy = adminUsername;
            application.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction("SeatApplications");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Update application status to Approved
            application.Status = "Approved";
            application.ProcessedDate = DateTime.UtcNow;
            application.ProcessedBy = adminUsername;
            application.AdminRemarks = "Approved by Hall Admin";
            application.UpdatedAt = DateTime.UtcNow;

            // Update seat status to Booked (locked)
            seat.Status = SeatStatuses.Booked;
            seat.BookedByStudentId = application.StudentId;
            seat.BookedOn = DateTime.UtcNow;
            seat.IsTemporarilyHeld = false;
            seat.HeldByStudentId = null;
            seat.HeldUntil = null;
            seat.UpdatedAt = DateTime.UtcNow;

            // Update room available slots
            if (seat.Room != null)
            {
                seat.Room.AvailableSlots = Math.Max(0, seat.Room.AvailableSlots - 1);
                if (seat.Room.AvailableSlots == 0)
                {
                    seat.Room.Status = "Occupied";
                }
            }

            // Auto-reject all other pending applications for this same seat
            var otherApplications = await _context.SeatApplications
                .Where(sa => sa.SeatId == seat.SeatId &&
                            sa.ApplicationId != applicationId &&
                            sa.Status == "Pending")
                .ToListAsync();

            foreach (var otherApp in otherApplications)
            {
                otherApp.Status = "Rejected";
                otherApp.AdminRemarks = "Seat was assigned to another student";
                otherApp.ProcessedDate = DateTime.UtcNow;
                otherApp.ProcessedBy = adminUsername;
                otherApp.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Application approved! {application.Student?.StudentName} has been assigned to {seat.SeatLabel} in Room {seat.Room?.RoomNumber}.";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Error approving application: {ex.Message}";
        }

        return RedirectToAction("SeatApplications");
    }

    /// <summary>
    /// Reject a seat application and release the seat
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RejectSeatApplication(int applicationId, string remarks)
    {
        if (!IsHallAdmin()) return RedirectToAction("Login", "Account");

        var adminUsername = GetHallAdminUsername();
        if (string.IsNullOrEmpty(adminUsername))
            return Unauthorized();

        var hallId = await GetHallAdminHallIdAsync();
        if (hallId == null)
            return Unauthorized();

        // Get the application with seat info
        var application = await _context.SeatApplications
            .Include(sa => sa.Seat)
            .Include(sa => sa.Student)
            .FirstOrDefaultAsync(sa => sa.ApplicationId == applicationId);

        if (application == null)
        {
            TempData["Error"] = "Application not found.";
            return RedirectToAction("SeatApplications");
        }

        // Verify it's for the admin's hall
        if (application.HallId != hallId.Value)
        {
            TempData["Error"] = "Unauthorized: This application is not for your hall.";
            return RedirectToAction("SeatApplications");
        }

        // Check if application is still pending
        if (application.Status != "Pending")
        {
            TempData["Error"] = $"This application has already been {application.Status.ToLower()}.";
            return RedirectToAction("SeatApplications");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Update application status to Rejected
            application.Status = "Rejected";
            application.ProcessedDate = DateTime.UtcNow;
            application.ProcessedBy = adminUsername;
            application.AdminRemarks = string.IsNullOrWhiteSpace(remarks) ? "Rejected by Hall Admin" : remarks;
            application.UpdatedAt = DateTime.UtcNow;

            // Release the seat back to Available if it was pending for this student
            var seat = application.Seat;
            if (seat != null && seat.BookedByStudentId == application.StudentId &&
                (seat.Status == SeatStatuses.Pending || seat.Status == SeatStatuses.Reserved))
            {
                seat.Status = SeatStatuses.Available;
                seat.BookedByStudentId = null;
                seat.BookedOn = null;
                seat.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Application from {application.Student?.StudentName ?? application.StudentId} has been rejected.";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Error rejecting application: {ex.Message}";
        }

        return RedirectToAction("SeatApplications");
    }

    /// <summary>
    /// Get seat details for a specific room via AJAX
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetRoomSeats(string roomId, int hallId)
    {
        if (!IsHallAdmin()) return Unauthorized();

        var adminHallId = await GetHallAdminHallIdAsync();
        if (adminHallId == null || adminHallId.Value != hallId)
            return Unauthorized();

        var seats = await _context.Seats
            .Include(s => s.BookedByStudent)
            .Where(s => s.RoomId == roomId && s.HallId == hallId)
            .OrderBy(s => s.SeatNumber)
            .Select(s => new
            {
                s.SeatId,
                s.SeatNumber,
                s.SeatType,
                s.SeatLabel,
                s.Status,
                s.Position,
                BookedByStudentId = s.BookedByStudentId,
                BookedByStudentName = s.BookedByStudent != null ? s.BookedByStudent.StudentName : null,
                s.BookedOn,
                IsWindowSide = s.SeatType.StartsWith("WINDOW"),
                IsDoorSide = s.SeatType.StartsWith("DOOR"),
                IsLeftPosition = s.SeatType.EndsWith("LEFT"),
                IsRightPosition = s.SeatType.EndsWith("RIGHT")
            })
            .ToListAsync();

        return Json(seats);
    }
}
