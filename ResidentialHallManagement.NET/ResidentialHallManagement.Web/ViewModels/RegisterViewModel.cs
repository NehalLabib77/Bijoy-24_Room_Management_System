namespace ResidentialHallManagement.Web.ViewModels;

public class RegisterViewModel
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserType { get; set; } = "Student";
    public string? StudentId { get; set; }
    public string? BoarderNo { get; set; }
    public string? FullName { get; set; }
    public string? Faculty { get; set; }
    public int? Semester { get; set; }
    public int? HallId { get; set; }
    public string? RegistrationToken { get; set; } // For Hall Admin registration
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
