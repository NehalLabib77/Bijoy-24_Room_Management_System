using ResidentialHallManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResidentialHallManagement.Core.Interfaces
{
    public interface IRoomChangeRequestService
    {
        Task<IEnumerable<RoomChangeRequest>> GetAllAsync();
        Task<RoomChangeRequest?> GetByIdAsync(int id);
        Task<IEnumerable<RoomChangeRequest>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<RoomChangeRequest>> GetPendingRequestsAsync();
        Task<IEnumerable<RoomChangeRequest>> GetByHallIdAsync(int hallId);
        Task<bool> HasPendingRequestAsync(string studentId);
        Task<RoomChangeRequest> CreateAsync(RoomChangeRequest request);
        Task<bool> ApproveRequestAsync(int requestId, string adminRemarks);
        Task<bool> RejectRequestAsync(int requestId, string adminRemarks);
        Task<bool> DeleteAsync(int id);
    }
}
