using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResidentialHallManagement.Data.Services
{
    public class RoomChangeRequestService : IRoomChangeRequestService
    {
        private readonly HallManagementDbContext _context;
        private readonly IRoomManagementService _roomManagementService;

        public RoomChangeRequestService(HallManagementDbContext context, IRoomManagementService roomManagementService)
        {
            _context = context;
            _roomManagementService = roomManagementService;
        }

        public async Task<IEnumerable<RoomChangeRequest>> GetAllAsync()
        {
            return await _context.RoomChangeRequests
                .Include(r => r.Student)
                .Include(r => r.RequestedRoom)
                .ThenInclude(room => room!.Hall)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<RoomChangeRequest?> GetByIdAsync(int id)
        {
            return await _context.RoomChangeRequests
                .Include(r => r.Student)
                .Include(r => r.RequestedRoom)
                .ThenInclude(room => room!.Hall)
                .FirstOrDefaultAsync(r => r.RequestId == id);
        }

        public async Task<IEnumerable<RoomChangeRequest>> GetByStudentIdAsync(string studentId)
        {
            return await _context.RoomChangeRequests
                .Include(r => r.RequestedRoom)
                .ThenInclude(room => room!.Hall)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomChangeRequest>> GetPendingRequestsAsync()
        {
            return await _context.RoomChangeRequests
                .Include(r => r.Student)
                .Include(r => r.RequestedRoom)
                .ThenInclude(room => room!.Hall)
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomChangeRequest>> GetByHallIdAsync(int hallId)
        {
            return await _context.RoomChangeRequests
                .Include(r => r.Student)
                .Include(r => r.RequestedRoom)
                .Where(r => r.RequestedHallId == hallId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<bool> HasPendingRequestAsync(string studentId)
        {
            return await _context.RoomChangeRequests
                .AnyAsync(r => r.StudentId == studentId && r.Status == "Pending");
        }

        public async Task<RoomChangeRequest> CreateAsync(RoomChangeRequest request)
        {
            request.RequestDate = DateTime.Now;
            request.Status = "Pending";

            _context.RoomChangeRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> ApproveRequestAsync(int requestId, string adminRemarks)
        {
            var request = await _context.RoomChangeRequests
                .Include(r => r.Student)
                .Include(r => r.RequestedRoom)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null || request.Status != "Pending")
                return false;

            // Update the room availability
            var requestedRoom = request.RequestedRoom;
            if (requestedRoom == null)
                return false;
            if (requestedRoom.Status == "Occupied")
                return false; // Room is no longer available

            // Get student's current room and make it available
            var currentAssignment = await _context.AssignedRooms
                .Include(ar => ar.Room)
                .Where(ar => ar.StudentId == request.StudentId)
                .OrderByDescending(ar => ar.StartDate)
                .FirstOrDefaultAsync();

            if (currentAssignment != null && currentAssignment.Room != null)
            {
                currentAssignment.Room.Status = "Available";
                currentAssignment.EndDate = DateTime.Now;
                // Increase available slots when current assignment freed
                var curRoom = currentAssignment.Room;
                if (curRoom != null)
                {
                    curRoom.AvailableSlots = curRoom.AvailableSlots + 1;
                }
            }

            // Assign new room via RoomManagementService (updates available slots)
            var requestedRoomIdentity = $"{requestedRoom.RoomNumber}/{requestedRoom.Block}";
            var assignment = await _roomManagementService.AssignRoomAsync(request.StudentId, request.RequestedHallId, requestedRoomIdentity, null, "Approved room change");
            if (assignment == null)
            {
                return false; // Could not assign (no slots)
            }

            // Update request status
            request.Status = "Approved";
            request.ProcessedDate = DateTime.Now;
            request.AdminRemarks = adminRemarks;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectRequestAsync(int requestId, string adminRemarks)
        {
            var request = await _context.RoomChangeRequests
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null || request.Status != "Pending")
                return false;

            request.Status = "Rejected";
            request.ProcessedDate = DateTime.Now;
            request.AdminRemarks = adminRemarks;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await _context.RoomChangeRequests.FindAsync(id);
            if (request == null)
                return false;

            _context.RoomChangeRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
