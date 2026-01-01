using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Core.Interfaces;

namespace ResidentialHallManagement.Data.Services;

public class RoomManagementService : IRoomManagementService
{
    private readonly HallManagementDbContext _context;

    public RoomManagementService(HallManagementDbContext context)
    {
        _context = context;
    }

    // Room Assignment Management
    public async Task<RoomAssignment?> AssignRoomAsync(string studentId, int hallId, string roomIdentity, string? bedNumber, string? notes)
    {
        // Check if student exists
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
            return null;

        // Check if hall exists
        var hall = await _context.Halls.FindAsync(hallId);
        if (hall == null)
            return null;

        // Check if student already has an active assignment
        var existingAssignment = await _context.RoomAssignments
            .FirstOrDefaultAsync(ra => ra.StudentId == studentId && ra.Status == "Active");
        if (existingAssignment != null)
            return null; // Student already has an active room

        // Parse room identity
        var parts = roomIdentity.Split('/');
        if (parts.Length < 2)
            return null; // Invalid room identity format

        var roomNumber = parts[0];
        var block = parts[1];

        // Find and validate the room
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.HallId == hallId && r.RoomNumber == roomNumber && r.Block == block);
        if (room == null || room.AvailableSlots <= 0)
            return null; // Room not found or no available slots

        // Create new assignment
        var assignment = new RoomAssignment
        {
            StudentId = studentId,
            HallId = hallId,
            RoomIdentity = roomIdentity,
            BedNumber = bedNumber,
            Notes = notes,
            AssignmentDate = DateTime.UtcNow,
            Status = "Active"
        };

        _context.RoomAssignments.Add(assignment);

        // Decrement available slots
        room.AvailableSlots = Math.Max(0, room.AvailableSlots - 1);
        if (room.AvailableSlots == 0)
        {
            room.Status = "Occupied";
        }
        _context.Rooms.Update(room);

        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<RoomAssignment?> GetRoomAssignmentAsync(int assignmentId)
    {
        return await _context.RoomAssignments
            .Include(ra => ra.Student)
            .Include(ra => ra.Hall)
            .FirstOrDefaultAsync(ra => ra.AssignmentId == assignmentId);
    }

    public async Task<List<RoomAssignment>> GetStudentRoomAssignmentsAsync(string studentId)
    {
        return await _context.RoomAssignments
            .Where(ra => ra.StudentId == studentId && ra.Status == "Active")
            .Include(ra => ra.Hall)
            .OrderByDescending(ra => ra.AssignmentDate)
            .ToListAsync();
    }

    public async Task<List<RoomAssignment>> GetHallRoomAssignmentsAsync(int hallId)
    {
        return await _context.RoomAssignments
            .Where(ra => ra.HallId == hallId && ra.Status == "Active")
            .Include(ra => ra.Student)
            .OrderByDescending(ra => ra.AssignmentDate)
            .ToListAsync();
    }

    public async Task<bool> RemoveRoomAssignmentAsync(int assignmentId)
    {
        var assignment = await _context.RoomAssignments.FindAsync(assignmentId);
        if (assignment == null)
            return false;

        assignment.Status = "Cancelled";
        assignment.ActualCheckout = DateTime.UtcNow;
        _context.RoomAssignments.Update(assignment);

        // Increase available slots for the room
        var parts = assignment.RoomIdentity?.Split('/');
        if (parts != null && parts.Length == 2)
        {
            var roomNumber = parts[0];
            var block = parts[1];
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.HallId == assignment.HallId && r.RoomNumber == roomNumber && r.Block == block);
            if (room != null)
            {
                room.AvailableSlots = room.AvailableSlots + 1;
                // Only set to Available if there are slots available and not under maintenance
                if (room.AvailableSlots > 0 && room.Status != "Maintenance")
                {
                    room.Status = "Available";
                }
                _context.Rooms.Update(room);
            }
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRoomAssignmentAsync(int assignmentId, string? bedNumber, string? notes)
    {
        var assignment = await _context.RoomAssignments.FindAsync(assignmentId);
        if (assignment == null)
            return false;

        if (!string.IsNullOrEmpty(bedNumber))
            assignment.BedNumber = bedNumber;
        if (notes != null)
            assignment.Notes = notes;

        _context.RoomAssignments.Update(assignment);
        await _context.SaveChangesAsync();
        return true;
    }

    // Room Application Management
    public async Task<RoomApplicationRequest?> CreateApplicationAsync(string studentId, int hallId, string requestedRoomIdentity, string? preferredSide = null)
    {
        // Check if student exists
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
            return null;

        // Check if hall exists
        var hall = await _context.Halls.FindAsync(hallId);
        if (hall == null)
            return null;

        var application = new RoomApplicationRequest
        {
            StudentId = studentId,
            HallId = hallId,
            RequestedRoomIdentity = requestedRoomIdentity,
            PreferredSide = preferredSide,
            ApplicationDate = DateTime.UtcNow,
            Status = "Pending"
        };

        _context.RoomApplicationRequests.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<RoomApplicationRequest?> GetApplicationAsync(int applicationId)
    {
        return await _context.RoomApplicationRequests
            .Include(rar => rar.Student)
            .Include(rar => rar.Hall)
            .FirstOrDefaultAsync(rar => rar.ApplicationId == applicationId);
    }

    public async Task<List<RoomApplicationRequest>> GetStudentApplicationsAsync(string studentId)
    {
        return await _context.RoomApplicationRequests
            .Where(rar => rar.StudentId == studentId)
            .Include(rar => rar.Hall)
            .OrderByDescending(rar => rar.ApplicationDate)
            .ToListAsync();
    }

    public async Task<List<RoomApplicationRequest>> GetPendingApplicationsAsync(int hallId)
    {
        return await _context.RoomApplicationRequests
            .Where(rar => rar.HallId == hallId && rar.Status == "Pending")
            .Include(rar => rar.Student)
            .OrderByDescending(rar => rar.ApplicationDate)
            .ToListAsync();
    }

    public async Task<bool> ApproveApplicationAsync(int applicationId, string approvedByUserId, string? remarks)
    {
        var application = await _context.RoomApplicationRequests
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

        if (application == null || application.Student == null)
            return false;

        // First, approve the application
        application.Status = "Approved";
        application.ApprovalDate = DateTime.UtcNow;
        application.ApprovedBy = approvedByUserId;
        application.AdminRemarks = remarks;

        // Automatically assign the room
        var assignment = await AssignRoomAsync(
            application.StudentId,
            application.HallId,
            application.RequestedRoomIdentity,
            null,
            $"Auto-assigned from approved application #{applicationId}"
        );

        if (assignment != null)
        {
            application.Status = "Assigned";
            _context.RoomApplicationRequests.Update(application);
            await _context.SaveChangesAsync();
            return true;
        }

        // If assignment fails, still approve but don't auto-assign
        _context.RoomApplicationRequests.Update(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectApplicationAsync(int applicationId, string rejectedByUserId, string remarks)
    {
        var application = await _context.RoomApplicationRequests.FindAsync(applicationId);
        if (application == null)
            return false;

        application.Status = "Rejected";
        application.ApprovalDate = DateTime.UtcNow;
        application.ApprovedBy = rejectedByUserId;
        application.AdminRemarks = remarks;

        _context.RoomApplicationRequests.Update(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignFromApplicationAsync(int applicationId, string? bedNumber)
    {
        var application = await _context.RoomApplicationRequests.FindAsync(applicationId);
        if (application == null || application.Status != "Approved")
            return false;

        // Create room assignment from approved application
        var assignment = await AssignRoomAsync(
            application.StudentId,
            application.HallId,
            application.RequestedRoomIdentity,
            bedNumber,
            $"Assigned from application {applicationId}"
        );

        if (assignment != null)
        {
            application.Status = "Assigned";
            _context.RoomApplicationRequests.Update(application);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    // Room Availability
    public async Task<List<string>> GetAvailableRoomsAsync(int hallId)
    {
        // Get all rooms for the hall
        var hallRooms = await _context.Rooms
            .Where(r => r.HallId == hallId && r.Status == "Available")
            .Select(r => $"{r.RoomNumber}/{r.Block}")
            .ToListAsync();

        // Filter out fully occupied rooms
        var availableRooms = new List<string>();
        foreach (var roomId in hallRooms)
        {
            if (await IsRoomAvailableAsync(hallId, roomId))
            {
                availableRooms.Add(roomId);
            }
        }

        return availableRooms;
    }

    public async Task<bool> IsRoomAvailableAsync(int hallId, string roomIdentity)
    {
        // Parse room identity (format: "125/C" -> RoomNumber: 125, Block: C)
        var parts = roomIdentity.Split('/');
        if (parts.Length != 2)
            return false;

        var roomNumber = parts[0];
        var block = parts[1];

        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.HallId == hallId && r.RoomNumber == roomNumber && r.Block == block);

        if (room == null)
            return false;

        // Use available slots property if present, otherwise fallback to occupancy check
        if (room.AvailableSlots > 0)
            return true;

        var occupancy = await GetRoomOccupancyAsync(hallId, roomIdentity);
        return occupancy < room.RoomCapacity;
    }

    public async Task<int> GetRoomOccupancyAsync(int hallId, string roomIdentity)
    {
        // Parse room identity
        var parts = roomIdentity.Split('/');
        if (parts.Length != 2)
            return 0;

        var roomNumber = parts[0];
        var block = parts[1];

        return await _context.RoomAssignments
            .Where(ra => ra.HallId == hallId && ra.RoomIdentity == roomIdentity && ra.Status == "Active")
            .CountAsync();
    }
}
