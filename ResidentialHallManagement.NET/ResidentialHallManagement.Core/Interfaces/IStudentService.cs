using ResidentialHallManagement.Core.Entities;

namespace ResidentialHallManagement.Core.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<IEnumerable<Student>> GetStudentsByHallIdAsync(int hallId);
    Task<Student?> GetStudentByIdAsync(string studentId);
    Task<Student> CreateStudentAsync(Student student);
    Task<Student> UpdateStudentAsync(Student student);
    Task<bool> DeleteStudentAsync(string studentId);
    Task<IEnumerable<Student>> SearchStudentsAsync(string? name = null, string? id = null, string? status = null);
    Task<bool> AssignHallToStudentAsync(string studentId, int hallId, string studentType);
    Task<IEnumerable<AssignedHall>> GetStudentHallHistoryAsync(string studentId);
}
