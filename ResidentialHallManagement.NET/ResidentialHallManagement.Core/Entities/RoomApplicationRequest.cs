using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class RoomApplicationRequest
{
    [Key]
    [Column("APPLICATION_ID")]
    public int ApplicationId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("STUDENT_ID")]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [Column("HALL_ID")]
    public int HallId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("REQUESTED_ROOM_IDENTITY")]
    public string RequestedRoomIdentity { get; set; } = string.Empty; // Format: "125/C"

    [MaxLength(20)]
    [Column("PREFERRED_SIDE")]
    public string? PreferredSide { get; set; } // Window or Door

    [Column("APPLICATION_DATE")]
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

    [MaxLength(20)]
    [Column("STATUS")]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Assigned

    [MaxLength(255)]
    [Column("ADMIN_REMARKS")]
    public string? AdminRemarks { get; set; }

    [Column("APPROVAL_DATE")]
    public DateTime? ApprovalDate { get; set; }

    [MaxLength(255)]
    [Column("APPROVED_BY")]
    public string? ApprovedBy { get; set; } // HallAdmin UserId

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }

    [ForeignKey("HallId")]
    public virtual Hall? Hall { get; set; }
}
