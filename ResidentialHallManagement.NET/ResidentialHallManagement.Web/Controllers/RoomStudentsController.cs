using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Data;
using ResidentialHallManagement.Web.ViewModels;

namespace ResidentialHallManagement.Web.Controllers;

[Authorize(Roles = "HallAdmin")]
public class RoomStudentsController : Controller
{
    private readonly HallManagementDbContext _context;

    public RoomStudentsController(HallManagementDbContext context)
    {
        _context = context;
    }

    // GET: RoomStudents/Index - List all rooms with student counts
    public async Task<IActionResult> Index(int? hallId)
    {
        var query = _context.Rooms
            .Include(r => r.Hall)
            .Include(r => r.AssignedRooms)
                .ThenInclude(ar => ar.Student)
            .AsQueryable();

        if (hallId.HasValue)
        {
            query = query.Where(r => r.HallId == hallId.Value);
        }

        var rooms = await query
            .Select(r => new RoomStudentViewModel
            {
                RoomId = r.RoomId,
                HallId = r.HallId,
                HallName = r.Hall != null ? r.Hall.HallName : "Unknown",
                RoomName = r.RoomName ?? $"Room {r.RoomId}",
                RoomNumber = r.RoomNumber,
                Wing = r.Wing,
                Block = r.Block,
                Floor = r.Floor,
                Capacity = r.RoomCapacity,
                AvailableSlots = r.AvailableSlots,
                OccupiedSlots = r.RoomCapacity - r.AvailableSlots,
                Status = r.Status,
                Students = r.AssignedRooms
                    .Where(ar => ar.AllocationStatus == "Active")
                    .Select(ar => new StudentInRoomViewModel
                    {
                        StudentId = ar.StudentId,
                        StudentName = ar.Student.StudentName,
                        BoarderNo = ar.Student.BoarderNo,
                        Faculty = ar.Student.Faculty,
                        Semester = ar.Student.Semester,
                        BedNumber = ar.BedNumber,
                        StartDate = ar.StartDate,
                        Mobile = ar.Student.Mobile,
                        Email = ar.Student.Email
                    }).ToList()
            })
            .OrderBy(r => r.HallName)
            .ThenBy(r => r.RoomNumber)
            .ToListAsync();

        var halls = await _context.Halls.ToListAsync();
        ViewBag.Halls = halls;
        ViewBag.SelectedHallId = hallId;

        return View(rooms);
    }

    // GET: RoomStudents/Details/roomId/hallId - View students in a specific room
    public async Task<IActionResult> Details(string roomId, int hallId)
    {
        if (string.IsNullOrEmpty(roomId))
            return BadRequest();

        var room = await _context.Rooms
            .Include(r => r.Hall)
            .Include(r => r.AssignedRooms)
                .ThenInclude(ar => ar.Student)
            .FirstOrDefaultAsync(r => r.RoomId == roomId && r.HallId == hallId);

        if (room == null)
            return NotFound();

        var viewModel = new RoomStudentViewModel
        {
            RoomId = room.RoomId,
            HallId = room.HallId,
            HallName = room.Hall?.HallName ?? "Unknown",
            RoomName = room.RoomName ?? $"Room {room.RoomId}",
            RoomNumber = room.RoomNumber,
            Wing = room.Wing,
            Block = room.Block,
            Floor = room.Floor,
            Capacity = room.RoomCapacity,
            AvailableSlots = room.AvailableSlots,
            OccupiedSlots = room.RoomCapacity - room.AvailableSlots,
            Status = room.Status,
            Students = room.AssignedRooms
                .Where(ar => ar.AllocationStatus == "Active")
                .Select(ar => new StudentInRoomViewModel
                {
                    StudentId = ar.StudentId,
                    StudentName = ar.Student.StudentName,
                    BoarderNo = ar.Student.BoarderNo,
                    Faculty = ar.Student.Faculty,
                    Semester = ar.Student.Semester,
                    Department = ar.Student.Department,
                    BedNumber = ar.BedNumber,
                    StartDate = ar.StartDate,
                    Mobile = ar.Student.Mobile,
                    Email = ar.Student.Email,
                    BloodGroup = ar.Student.BloodGroup,
                    EmergencyContactName = ar.Student.EmergencyContactName,
                    EmergencyContactPhone = ar.Student.EmergencyContactPhone
                }).ToList()
        };

        return View(viewModel);
    }

    // POST: RoomStudents/RemoveStudent - Remove a student from a room
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveStudent(string studentId, string roomId, int hallId)
    {
        if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(roomId))
            return BadRequest();

        var assignedRoom = await _context.AssignedRooms
            .FirstOrDefaultAsync(ar => ar.StudentId == studentId &&
                                       ar.RoomId == roomId &&
                                       ar.AssignedHallId == hallId &&
                                       ar.AllocationStatus == "Active");

        if (assignedRoom == null)
            return NotFound();

        try
        {
            // Change allocation status to Completed
            assignedRoom.AllocationStatus = "Completed";
            assignedRoom.EndDate = DateTime.Now;

            // Increment available slots in the room
            var room = await _context.Rooms.FindAsync(roomId, hallId);
            if (room != null)
            {
                room.AvailableSlots += 1;
                if (room.AvailableSlots > 0)
                {
                    room.Status = "Available";
                }
                _context.Rooms.Update(room);
            }

            _context.AssignedRooms.Update(assignedRoom);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Student {studentId} has been removed from the room successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error removing student: {ex.Message}";
        }

        return RedirectToAction("Details", new { roomId, hallId });
    }
}
