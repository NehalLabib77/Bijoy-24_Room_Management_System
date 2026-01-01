using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class Room
{
    [Key]
    [Column("R_ID", Order = 0)]
    [MaxLength(4)]
    public string RoomId { get; set; } = string.Empty;

    [Key]
    [Column("H_ID", Order = 1)]
    public int HallId { get; set; }

    [Required]
    [Column("R_CAPACITY")]
    public int RoomCapacity { get; set; }

    [Column("R_AVAILABLE")]
    public int AvailableSlots { get; set; }

    [MaxLength(50)]
    [Column("R_NAME")]
    public string? RoomName { get; set; }

    [MaxLength(8)]
    [Column("R_WING")]
    public string? Wing { get; set; }

    [MaxLength(10)]
    [Column("R_BLOCK")]
    public string? Block { get; set; }

    [Column("R_FLOOR")]
    public int? Floor { get; set; }

    [MaxLength(10)]
    [Column("R_NUMBER")]
    public string? RoomNumber { get; set; }

    [MaxLength(20)]
    [Column("R_STATUS")]
    public string Status { get; set; } = "Available"; // Available/Occupied/Maintenance

    // Navigation properties
    [ForeignKey("HallId")]
    public virtual Hall? Hall { get; set; }

    public virtual ICollection<AssignedRoom> AssignedRooms { get; set; } = new List<AssignedRoom>();
}
