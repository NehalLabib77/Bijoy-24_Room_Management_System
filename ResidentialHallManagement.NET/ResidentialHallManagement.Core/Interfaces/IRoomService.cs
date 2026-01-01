using ResidentialHallManagement.Core.Entities;

namespace ResidentialHallManagement.Core.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<Room>> GetRoomsByHallIdAsync(int hallId);
    Task<Room?> GetRoomByIdAsync(string roomId, int hallId);
    Task<Room> CreateRoomAsync(Room room);
    Task<Room> UpdateRoomAsync(Room room);
    Task<bool> DeleteRoomAsync(string roomId, int hallId);
    Task<bool> AssignRoomToStudentAsync(string studentId, int hallId, string roomId, DateTime startDate);
    Task<IEnumerable<AssignedRoom>> GetStudentRoomHistoryAsync(string studentId);
    Task<IEnumerable<Student>> GetStudentsInRoomAsync(string roomId, int hallId);
}
