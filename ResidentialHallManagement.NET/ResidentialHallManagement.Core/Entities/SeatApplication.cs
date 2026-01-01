using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

/// <summary>
/// Represents a student's application for a specific seat in a room.
/// Tracks the complete lifecycle from application to approval/rejection.
/// </summary>
public class SeatApplication
{
    [Key]
    [Column("APPLICATION_ID")]
    public int ApplicationId { get; set; }

    [Required]
    [MaxLength(10)]
    [Column("STUDENT_ID")]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [Column("SEAT_ID")]
    public int SeatId { get; set; }

    [Required]
    [Column("HALL_ID")]
    public int HallId { get; set; }

    [Required]
    [MaxLength(4)]
    [Column("ROOM_ID")]
    public string RoomId { get; set; } = string.Empty;

    [Column("APPLICATION_DATE")]
    public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(20)]
    [Column("STATUS")]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled, Expired

    [MaxLength(500)]
    [Column("STUDENT_REMARKS")]
    public string? StudentRemarks { get; set; }

    [MaxLength(500)]
    [Column("ADMIN_REMARKS")]
    public string? AdminRemarks { get; set; }

    [Column("PROCESSED_DATE")]
    public DateTime? ProcessedDate { get; set; }

    [MaxLength(255)]
    [Column("PROCESSED_BY")]
    public string? ProcessedBy { get; set; } // Admin who processed the application

    [MaxLength(50)]
    [Column("ACADEMIC_YEAR")]
    public string? AcademicYear { get; set; }

    [MaxLength(20)]
    [Column("SEMESTER")]
    public string? Semester { get; set; }

    [Column("PRIORITY_SCORE")]
    public int PriorityScore { get; set; } = 0; // For priority-based allocation

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("UPDATED_AT")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }

    [ForeignKey("SeatId")]
    public virtual Seat? Seat { get; set; }

    [ForeignKey("HallId")]
    public virtual Hall? Hall { get; set; }
}
