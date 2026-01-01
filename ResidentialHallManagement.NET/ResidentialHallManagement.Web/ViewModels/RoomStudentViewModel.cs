namespace ResidentialHallManagement.Web.ViewModels;

public class RoomStudentViewModel
{
    public string RoomId { get; set; } = string.Empty;
    public int HallId { get; set; }
    public string HallName { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string? RoomNumber { get; set; }
    public string? Wing { get; set; }
    public string? Block { get; set; }
    public int? Floor { get; set; }
    public int Capacity { get; set; }
    public int AvailableSlots { get; set; }
    public int OccupiedSlots { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<StudentInRoomViewModel> Students { get; set; } = new();
}

public class StudentInRoomViewModel
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string BoarderNo { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public int Semester { get; set; }
    public string? Department { get; set; }
    public string? BedNumber { get; set; }
    public DateTime StartDate { get; set; }
    public string Mobile { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BloodGroup { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}
