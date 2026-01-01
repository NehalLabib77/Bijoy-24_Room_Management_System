using ResidentialHallManagement.Core.Entities;

namespace ResidentialHallManagement.Core.Interfaces;

public interface IRoomManagementService
{
    // Room Assignment Management
    Task<RoomAssignment?> AssignRoomAsync(string studentId, int hallId, string roomIdentity, string? bedNumber, string? notes);
    Task<RoomAssignment?> GetRoomAssignmentAsync(int assignmentId);
    Task<List<RoomAssignment>> GetStudentRoomAssignmentsAsync(string studentId);
    Task<List<RoomAssignment>> GetHallRoomAssignmentsAsync(int hallId);
    Task<bool> RemoveRoomAssignmentAsync(int assignmentId);
    Task<bool> UpdateRoomAssignmentAsync(int assignmentId, string? bedNumber, string? notes);

    // Room Application Management
    Task<RoomApplicationRequest?> CreateApplicationAsync(string studentId, int hallId, string requestedRoomIdentity, string? preferredSide = null);
    Task<RoomApplicationRequest?> GetApplicationAsync(int applicationId);
    Task<List<RoomApplicationRequest>> GetStudentApplicationsAsync(string studentId);
    Task<List<RoomApplicationRequest>> GetPendingApplicationsAsync(int hallId);
    Task<bool> ApproveApplicationAsync(int applicationId, string approvedByUserId, string? remarks);
    Task<bool> RejectApplicationAsync(int applicationId, string rejectedByUserId, string remarks);
    Task<bool> AssignFromApplicationAsync(int applicationId, string? bedNumber);

    // Room Availability
    Task<List<string>> GetAvailableRoomsAsync(int hallId);
    Task<bool> IsRoomAvailableAsync(int hallId, string roomIdentity);
    Task<int> GetRoomOccupancyAsync(int hallId, string roomIdentity);
}
