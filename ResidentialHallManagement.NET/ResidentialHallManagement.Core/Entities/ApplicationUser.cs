using Microsoft.AspNetCore.Identity;

namespace ResidentialHallManagement.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string? StudentId { get; set; }
    public int? HallId { get; set; }
    public string UserType { get; set; } = string.Empty; // Admin, HallAdmin, Student
    public string FullName { get; set; } = string.Empty;
}
