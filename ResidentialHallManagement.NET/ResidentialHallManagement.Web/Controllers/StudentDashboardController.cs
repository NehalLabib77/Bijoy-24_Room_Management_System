using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Core.Interfaces;
using ResidentialHallManagement.Data;
using ResidentialHallManagement.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ResidentialHallManagement.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly HallManagementDbContext _context;
        private readonly IRoomChangeRequestService _roomChangeService;
        private readonly IRoomManagementService _roomManagementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentDashboardController(
            HallManagementDbContext context,
            IRoomChangeRequestService roomChangeService,
            IRoomManagementService roomManagementService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roomChangeService = roomChangeService;
            _roomManagementService = roomManagementService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                ViewBag.Error = "Student ID not found in your profile.";
                return View();
            }
            if (string.IsNullOrEmpty(studentId))
            {
                ViewBag.Error = "Student ID not found in your profile.";
                return View();
            }

            // Get comprehensive student information
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                ViewBag.Error = "Student record not found.";
                return View();
            }

            // Get current room assignment using RoomAssignment
            var currentAllocation = await _context.RoomAssignments
                .Include(ra => ra.Hall)
                .Include(ra => ra.Student)
                .Where(ra => ra.StudentId == studentId && ra.Status == "Active")
                .OrderByDescending(ra => ra.AssignmentDate)
                .FirstOrDefaultAsync();

            // Fetch Room data separately if allocation exists
            Room? roomData = null;
            if (currentAllocation != null)
            {
                // Extract room number from RoomIdentity (format: "125/C")
                var roomIdentityParts = currentAllocation.RoomIdentity.Split('/');
                if (roomIdentityParts.Length > 0)
                {
                    roomData = await _context.Rooms
                        .Include(r => r.Hall)
                        .FirstOrDefaultAsync(r => r.RoomNumber == roomIdentityParts[0] && r.HallId == currentAllocation.HallId);
                }
            }

            // Get maintenance requests
            var maintenanceRequests = await _context.MaintenanceRequests
                .Include(m => m.Room)
                .ThenInclude(r => r!.Hall)
                .Where(m => m.StudentId == studentId)
                .OrderByDescending(m => m.SubmittedOn)
                .Take(5)
                .ToListAsync();

            // Get room change requests
            var roomChangeRequests = await _roomChangeService.GetByStudentIdAsync(studentId);

            // Check for pending room change request
            var hasPendingRequest = await _roomChangeService.HasPendingRequestAsync(studentId);

            ViewBag.Student = student;
            ViewBag.CurrentAllocation = currentAllocation;
            ViewBag.RoomData = roomData;
            ViewBag.MaintenanceRequests = maintenanceRequests;
            ViewBag.PendingMaintenanceCount = maintenanceRequests.Count(m => m.Status == "Pending");
            ViewBag.RoomChangeRequests = roomChangeRequests;
            ViewBag.HasPendingRequest = hasPendingRequest;

            // Get seat booking information
            var bookedSeat = await _context.Seats
                .Include(s => s.Room)
                .ThenInclude(r => r!.Hall)
                .FirstOrDefaultAsync(s => s.BookedByStudentId == studentId && s.Status == "Booked");

            var seatApplication = await _context.SeatApplications
                .Include(sa => sa.Seat)
                .ThenInclude(s => s!.Room)
                .ThenInclude(r => r!.Hall)
                .Where(sa => sa.StudentId == studentId)
                .OrderByDescending(sa => sa.ApplicationDate)
                .FirstOrDefaultAsync();

            ViewBag.BookedSeat = bookedSeat;
            ViewBag.SeatApplication = seatApplication;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ApplyRoomChange()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found in your profile.";
                return RedirectToAction("Index");
            }

            // Check if student already has a pending request
            var hasPending = await _roomChangeService.HasPendingRequestAsync(studentId);
            if (hasPending)
            {
                TempData["Error"] = "You already have a pending room change request.";
                return RedirectToAction("Index");
            }

            // Get available rooms
            var availableRooms = await _context.Rooms
                .Include(r => r.Hall)
                .Where(r => r.Status == "Available")
                .ToListAsync();

            ViewBag.AvailableRooms = availableRooms;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyRoomChange(string requestedRoomId, int requestedHallId, string reason)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found in your profile.";
                return RedirectToAction("Index");
            }

            // Check if student already has a pending request
            var hasPending = await _roomChangeService.HasPendingRequestAsync(studentId);
            if (hasPending)
            {
                TempData["Error"] = "You already have a pending room change request.";
                return RedirectToAction("Index");
            }

            // Verify room exists and is available
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == requestedRoomId && r.HallId == requestedHallId);

            if (room == null)
            {
                TempData["Error"] = "Selected room not found.";
                return RedirectToAction("ApplyRoomChange");
            }

            if (room.Status != "Available")
            {
                TempData["Error"] = "Selected room is no longer available.";
                return RedirectToAction("ApplyRoomChange");
            }

            // Create request
            var request = new RoomChangeRequest
            {
                StudentId = studentId,
                RequestedRoomId = requestedRoomId,
                RequestedHallId = requestedHallId,
                Reason = reason
            };

            await _roomChangeService.CreateAsync(request);

            TempData["Success"] = "Your room change request has been submitted successfully.";
            return RedirectToAction("Index");
        }

        // Room Application
        [HttpGet]
        public async Task<IActionResult> ApplyForRoom()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found in your profile.";
                return RedirectToAction("Index");
            }

            // Check if student already has an active room
            var hasActiveRoom = await _context.RoomAssignments
                .AnyAsync(ra => ra.StudentId == studentId && ra.Status == "Active");

            if (hasActiveRoom)
            {
                TempData["Error"] = "You already have an active room allocation. Please apply for room change instead.";
                return RedirectToAction("Index");
            }

            // Get available rooms
            var availableRooms = await _context.Rooms
                .Include(r => r.Hall)
                .Where(r => r.Status == "Available")
                .ToListAsync();

            ViewBag.AvailableRooms = availableRooms;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyForRoom(string roomId, int hallId, string bedNumber, string preferredSide)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found in your profile.";
                return RedirectToAction("Index");
            }

            // Validate preferred side/seat type
            var validSeatTypes = new[] { "Window", "Door", "WINDOW_LEFT", "WINDOW_RIGHT", "DOOR_LEFT", "DOOR_RIGHT" };
            if (string.IsNullOrEmpty(preferredSide) || !validSeatTypes.Contains(preferredSide))
            {
                TempData["Error"] = "Please select a valid seat preference.";
                return RedirectToAction("ApplyForRoom");
            }

            // Map new seat types to display names for the success message
            var seatDisplayName = preferredSide switch
            {
                "WINDOW_LEFT" => "Seat 1 (Window Left)",
                "WINDOW_RIGHT" => "Seat 2 (Window Right)",
                "DOOR_LEFT" => "Seat 3 (Door Left)",
                "DOOR_RIGHT" => "Seat 4 (Door Right)",
                "Window" => "Window Side",
                "Door" => "Door Side",
                _ => preferredSide
            };

            // Verify room exists and is available
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == roomId && r.HallId == hallId);

            if (room == null || room.Status != "Available")
            {
                TempData["Error"] = "Selected room is not available.";
                return RedirectToAction("ApplyForRoom");
            }

            // Create room application request so hall admin can approve
            var requestedRoomIdentity = $"{room.RoomNumber}/{room.Block}";
            var application = await _roomManagementService.CreateApplicationAsync(studentId!, hallId, requestedRoomIdentity, preferredSide);
            if (application != null)
            {
                TempData["Success"] = $"Your room application for {seatDisplayName} has been submitted and is pending approval.";
            }
            else
            {
                TempData["Error"] = "Failed to submit room application.";
            }
            return RedirectToAction("Index");
        }

        // Maintenance Requests
        [HttpGet]
        public async Task<IActionResult> MaintenanceRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToAction("Index");
            }

            var requests = await _context.MaintenanceRequests
                .Include(m => m.Room)
                .ThenInclude(r => r!.Hall)
                .Where(m => m.StudentId == studentId)
                .OrderByDescending(m => m.SubmittedOn)
                .ToListAsync();

            return View(requests);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMaintenanceRequest()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;

            // Get student's current room
            var currentRoom = await _context.AssignedRooms
                .Include(ar => ar.Room)
                .ThenInclude(r => r!.Hall)
                .Where(ar => ar.StudentId == studentId && ar.AllocationStatus == "Active")
                .OrderByDescending(ar => ar.StartDate)
                .FirstOrDefaultAsync();

            if (currentRoom == null)
            {
                TempData["Error"] = "You must have an active room allocation to submit maintenance requests.";
                return RedirectToAction("Index");
            }

            ViewBag.CurrentRoom = currentRoom;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaintenanceRequest(string issue, string priority)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;

            // Get student's current room
            var currentRoom = await _context.AssignedRooms
                .Where(ar => ar.StudentId == studentId && ar.AllocationStatus == "Active")
                .OrderByDescending(ar => ar.StartDate)
                .FirstOrDefaultAsync();

            if (currentRoom == null)
            {
                TempData["Error"] = "No active room allocation found.";
                return RedirectToAction("Index");
            }

            var maintenanceRequest = new MaintenanceRequest
            {
                StudentId = studentId!,
                RoomId = currentRoom.RoomId,
                HallId = currentRoom.AssignedHallId,
                Issue = issue,
                Priority = priority ?? "Medium",
                Status = "Pending",
                SubmittedOn = DateTime.Now
            };

            _context.MaintenanceRequests.Add(maintenanceRequest);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Maintenance request submitted successfully!";
            return RedirectToAction("MaintenanceRequests");
        }

        // Room Change Applications
        [HttpGet]
        public async Task<IActionResult> RoomChangeApplications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;

            var requests = await _roomChangeService.GetByStudentIdAsync(studentId!);

            return View(requests);
        }

        // View My Roommates
        [HttpGet]
        public async Task<IActionResult> MyRoommates()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found in your profile.";
                return RedirectToAction("Index");
            }

            // Get current student's room assignment
            var currentAssignment = await _context.AssignedRooms
                .Include(ar => ar.Room)
                    .ThenInclude(r => r!.Hall)
                .Include(ar => ar.Student)
                .Where(ar => ar.StudentId == studentId && ar.AllocationStatus == "Active")
                .OrderByDescending(ar => ar.StartDate)
                .FirstOrDefaultAsync();

            if (currentAssignment == null)
            {
                TempData["Error"] = "You don't have an active room allocation.";
                return RedirectToAction("Index");
            }

            // Get all roommates (students in the same room)
            var roommates = await _context.AssignedRooms
                .Include(ar => ar.Student)
                    .ThenInclude(s => s.BoarderRegistry)
                .Where(ar => ar.RoomId == currentAssignment.RoomId
                          && ar.AssignedHallId == currentAssignment.AssignedHallId
                          && ar.StudentId != studentId
                          && ar.AllocationStatus == "Active")
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
                })
                .ToListAsync();

            var viewModel = new RoomStudentViewModel
            {
                RoomId = currentAssignment.RoomId,
                HallId = currentAssignment.AssignedHallId,
                HallName = currentAssignment.Room?.Hall?.HallName ?? "Unknown",
                RoomName = currentAssignment.Room?.RoomName ?? $"Room {currentAssignment.RoomId}",
                RoomNumber = currentAssignment.Room?.RoomNumber,
                Wing = currentAssignment.Room?.Wing,
                Block = currentAssignment.Room?.Block,
                Floor = currentAssignment.Room?.Floor,
                Capacity = currentAssignment.Room?.RoomCapacity ?? 0,
                AvailableSlots = currentAssignment.Room?.AvailableSlots ?? 0,
                Students = roommates
            };

            // Add current student to the list for display
            ViewBag.CurrentStudent = new StudentInRoomViewModel
            {
                StudentId = currentAssignment.StudentId,
                StudentName = currentAssignment.Student.StudentName,
                BoarderNo = currentAssignment.Student.BoarderNo,
                Faculty = currentAssignment.Student.Faculty,
                Semester = currentAssignment.Student.Semester,
                Department = currentAssignment.Student.Department,
                BedNumber = currentAssignment.BedNumber,
                StartDate = currentAssignment.StartDate,
                Mobile = currentAssignment.Student.Mobile,
                Email = currentAssignment.Student.Email,
                BloodGroup = currentAssignment.Student.BloodGroup
            };

            return View(viewModel);
        }

        // New: View Room Members
        [HttpGet]
        public async Task<IActionResult> ViewRoomMembers()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentId = user.StudentId;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Student ID not found in your profile.";
                return RedirectToAction("Index");
            }

            // Get current student info
            var currentStudent = await _context.Students.FindAsync(studentId);
            if (currentStudent == null)
            {
                TempData["Error"] = "Student record not found.";
                return RedirectToAction("Index");
            }

            // Get student's current room assignment using RoomAssignment
            var currentAssignment = await _context.RoomAssignments
                .Include(ra => ra.Hall)
                .Where(ra => ra.StudentId == studentId && ra.Status == "Active")
                .OrderByDescending(ra => ra.AssignmentDate)
                .FirstOrDefaultAsync();

            if (currentAssignment == null)
            {
                TempData["Error"] = "You are not currently assigned to any room.";
                return RedirectToAction("Index");
            }

            // Get room details
            var roomIdentityParts = currentAssignment.RoomIdentity.Split('/');
            Room? roomData = null;
            if (roomIdentityParts.Length > 0)
            {
                roomData = await _context.Rooms
                    .Include(r => r.Hall)
                    .FirstOrDefaultAsync(r => r.RoomNumber == roomIdentityParts[0] && r.HallId == currentAssignment.HallId);
            }

            // Get all active students in the same room (excluding current student)
            var roommates = await _context.RoomAssignments
                .Where(ra => ra.RoomIdentity == currentAssignment.RoomIdentity
                            && ra.HallId == currentAssignment.HallId
                            && ra.Status == "Active"
                            && ra.StudentId != studentId)
                .Include(ra => ra.Student)
                .Select(ra => ra.Student)
                .Where(s => s != null)
                .ToListAsync();

            ViewBag.RoomIdentity = currentAssignment.RoomIdentity;
            ViewBag.HallId = currentAssignment.HallId;
            ViewBag.HallName = currentAssignment.Hall?.HallName;
            ViewBag.RoomData = roomData;
            ViewBag.CurrentStudent = currentStudent;
            ViewBag.CurrentAssignment = currentAssignment;

            return View(roommates);
        }
    }
}
