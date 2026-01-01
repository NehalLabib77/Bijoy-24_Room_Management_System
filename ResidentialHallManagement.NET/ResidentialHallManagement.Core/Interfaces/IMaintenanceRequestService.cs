using ResidentialHallManagement.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResidentialHallManagement.Core.Interfaces
{
    public interface IMaintenanceRequestService
    {
        Task<IEnumerable<MaintenanceRequest>> GetAllAsync();
        Task<MaintenanceRequest?> GetByIdAsync(int id);
        Task<IEnumerable<MaintenanceRequest>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<MaintenanceRequest>> GetPendingRequestsAsync();
        Task<MaintenanceRequest> CreateAsync(MaintenanceRequest request);
        Task<bool> UpdateStatusAsync(int id, string status, string? technicianNote = null);
        Task<bool> DeleteAsync(int id);
    }
}
