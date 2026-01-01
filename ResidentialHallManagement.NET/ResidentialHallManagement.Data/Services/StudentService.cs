using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Core.Interfaces;
using ResidentialHallManagement.Data;

namespace ResidentialHallManagement.Data.Services;

public class StudentService : IStudentService
{
    private readonly HallManagementDbContext _context;

    public StudentService(HallManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _context.Students
            .Include(s => s.AssignedHalls)
                .ThenInclude(ah => ah.Hall)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetStudentsByHallIdAsync(int hallId)
    {
        return await _context.Students
            .Where(s => s.AssignedHalls.Any(ah => ah.HallId == hallId && ah.IsCurrent == "YES"))
            .Include(s => s.AssignedHalls)
                .ThenInclude(ah => ah.Hall)
            .ToListAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(string studentId)
    {
        return await _context.Students
            .Include(s => s.AssignedHalls)
                .ThenInclude(ah => ah.Hall)
            .Include(s => s.AssignedRooms)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task<Student> CreateStudentAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateStudentAsync(Student student)
    {
        _context.Entry(student).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteStudentAsync(string studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Student>> SearchStudentsAsync(string? name = null, string? id = null, string? status = null)
    {
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(s => s.StudentName.Contains(name));
        }

        if (!string.IsNullOrEmpty(id))
        {
            query = query.Where(s => s.StudentId.Contains(id));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(s => s.Status == status.ToUpper());
        }

        return await query.Include(s => s.AssignedHalls)
            .ThenInclude(ah => ah.Hall)
            .ToListAsync();
    }

    public async Task<bool> AssignHallToStudentAsync(string studentId, int hallId, string studentType)
    {
        // Mark previous assignments as not current
        var previousAssignments = await _context.AssignedHalls
            .Where(ah => ah.StudentId == studentId && ah.IsCurrent == "YES")
            .ToListAsync();

        foreach (var assignment in previousAssignments)
        {
            assignment.IsCurrent = "NO";
        }

        var assignedHall = new AssignedHall
        {
            StudentId = studentId,
            HallId = hallId,
            AssignedStudentType = studentType.ToUpper(),
            IsCurrent = "YES"
        };

        _context.AssignedHalls.Add(assignedHall);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AssignedHall>> GetStudentHallHistoryAsync(string studentId)
    {
        return await _context.AssignedHalls
            .Where(ah => ah.StudentId == studentId)
            .Include(ah => ah.Hall)
            .OrderByDescending(ah => ah.IsCurrent)
            .ToListAsync();
    }
}
