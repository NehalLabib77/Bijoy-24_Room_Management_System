using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class RoomAssignment
{
    [Key]
    [Column("ASSIGNMENT_ID")]
    public int AssignmentId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("ROOM_IDENTITY")]
    public string RoomIdentity { get; set; } = string.Empty; // Format: "125/C" (RoomNumber/Block)

    [Required]
    [Column("HALL_ID")]
    public int HallId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("STUDENT_ID")]
    public string StudentId { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("BED_NUMBER")]
    public string? BedNumber { get; set; }

    [Column("ASSIGNMENT_DATE")]
    public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;

    [Column("EXPECTED_CHECKOUT")]
    public DateTime? ExpectedCheckout { get; set; }

    [Column("ACTUAL_CHECKOUT")]
    public DateTime? ActualCheckout { get; set; }

    [MaxLength(20)]
    [Column("STATUS")]
    public string Status { get; set; } = "Active"; // Active, CheckedOut, Cancelled

    [MaxLength(500)]
    [Column("NOTES")]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }

    [ForeignKey("HallId")]
    public virtual Hall? Hall { get; set; }
}
