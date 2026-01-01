using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class AssignedHall
{
    [Key]
    [Column("H_ID", Order = 1)]
    public int HallId { get; set; }

    [Key]
    [Column("S_ID", Order = 0)]
    [MaxLength(10)]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [MaxLength(8)]
    [Column("ASSIGNED_S_TYPE")]
    public string AssignedStudentType { get; set; } = string.Empty; // RESIDENT or ATTACHED

    [MaxLength(6)]
    [Column("ASSIGNED_CURRENT")]
    public string? IsCurrent { get; set; } = "YES";

    // Navigation properties
    [ForeignKey("HallId")]
    public virtual Hall Hall { get; set; } = null!;

    [ForeignKey("StudentId")]
    public virtual Student Student { get; set; } = null!;

    public virtual ICollection<AssignedRoom> AssignedRooms { get; set; } = new List<AssignedRoom>();
}
