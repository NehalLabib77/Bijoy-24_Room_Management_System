using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class AssignedRoom
{
    [Key]
    [Column("ASS_ROOM_SID")]
    public int AssignedRoomId { get; set; }

    [Column("S_ID")]
    [MaxLength(10)]
    public string StudentId { get; set; } = string.Empty;

    [Column("ASS_HALL_H_ID")]
    public int AssignedHallId { get; set; }

    [Column("R_ID")]
    [MaxLength(4)]
    public string RoomId { get; set; } = string.Empty;

    [Column("ASS_ROOM_SDATE")]
    public DateTime StartDate { get; set; }

    [Column("ASS_ROOM_EDATE")]
    public DateTime? EndDate { get; set; }

    [MaxLength(10)]
    [Column("BED_NUMBER")]
    public string? BedNumber { get; set; }

    [Column("EXPECTED_CHECKOUT")]
    public DateTime? ExpectedCheckout { get; set; }

    [Column("ACTUAL_CHECKOUT")]
    public DateTime? ActualCheckout { get; set; }

    [MaxLength(20)]
    [Column("ALLOCATION_STATUS")]
    public string AllocationStatus { get; set; } = "Active"; // Active, Completed, Cancelled

    // Navigation properties
    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("RoomId,AssignedHallId")]
    public virtual Room Room { get; set; } = null!;

    public virtual AssignedHall? AssignedHall { get; set; }
}
