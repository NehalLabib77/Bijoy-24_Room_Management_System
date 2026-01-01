using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class Student
{
    [Key]
    [Column("S_ID")]
    [MaxLength(10)]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [MaxLength(25)]
    [Column("S_NAME")]
    public string StudentName { get; set; } = string.Empty;

    [Required]
    [MaxLength(8)]
    [Column("S_GENDER")]
    public string Gender { get; set; } = string.Empty; // MALE or FEMALE

    [Required]
    [MaxLength(15)]
    [Column("S_MOBILE")]
    public string Mobile { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Column("S_BOARDER_NO")]
    public string BoarderNo { get; set; } = string.Empty;

    [Required]
    [MaxLength(3)]
    [Column("S_BLD_GRP")]
    public string BloodGroup { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("S_PRM_ADDR")]
    public string PermanentAddress { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("S_EMAIL")]
    public string? Email { get; set; }

    [MaxLength(50)]
    [Column("S_EMERGENCY_NAME")]
    public string? EmergencyContactName { get; set; }

    [MaxLength(15)]
    [Column("S_EMERGENCY_PHONE")]
    public string? EmergencyContactPhone { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("S_FACULTY")]
    public string Faculty { get; set; } = string.Empty;

    [Required]
    [Column("S_SEMESTER")]
    public int Semester { get; set; }

    [Required]
    [MaxLength(8)]
    [Column("S_STATUS")]
    public string Status { get; set; } = string.Empty; // RUNNING or ALUMNI

    [MaxLength(50)]
    [Column("S_FATHER_NAME")]
    public string? FatherName { get; set; }

    [MaxLength(50)]
    [Column("S_MOTHER_NAME")]
    public string? MotherName { get; set; }

    [MaxLength(30)]
    [Column("S_RELIGION")]
    public string? Religion { get; set; }

    [MaxLength(50)]
    [Column("S_DEPARTMENT")]
    public string? Department { get; set; }

    // Navigation properties
    // Student must be assigned a BoarderNo from BoarderRegistry
    public virtual BoarderRegistry? BoarderRegistry { get; set; }

    public virtual ICollection<AssignedHall> AssignedHalls { get; set; } = new List<AssignedHall>();
    public virtual ICollection<AssignedRoom> AssignedRooms { get; set; } = new List<AssignedRoom>();
    public virtual ICollection<RoomChangeRequest> RoomChangeRequests { get; set; } = new List<RoomChangeRequest>();
    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
}
