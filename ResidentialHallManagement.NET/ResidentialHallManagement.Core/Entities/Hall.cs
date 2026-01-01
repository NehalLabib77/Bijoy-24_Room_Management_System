using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class Hall
{
    [Key]
    [Column("H_ID")]
    public int HallId { get; set; }

    [Required]
    [MaxLength(25)]
    [Column("H_NAME")]
    public string HallName { get; set; } = string.Empty;

    [Required]
    [MaxLength(8)]
    [Column("H_TYPE")]
    public string HallType { get; set; } = string.Empty; // MALE or FEMALE

    [Column("H_CAPACITY")]
    public int HallCapacity { get; set; }

    [MaxLength(100)]
    [Column("H_LOCATION")]
    public string? Location { get; set; }

    // Navigation properties
    public virtual ICollection<AssignedHall> AssignedHalls { get; set; } = new List<AssignedHall>();
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    public virtual ICollection<HallAdmin> HallAdmins { get; set; } = new List<HallAdmin>();
}
