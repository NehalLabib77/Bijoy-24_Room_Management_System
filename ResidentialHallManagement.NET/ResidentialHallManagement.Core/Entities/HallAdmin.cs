using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class HallAdmin
{
    [Key]
    [Column("HALL_ADMIN_ID")]
    public int HallAdminId { get; set; }

    [Required]
    [Column("HALL_ID")]
    public int HallId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("USER_ID")]
    public string UserId { get; set; } = string.Empty; // Links to ApplicationUser

    [Required]
    [MaxLength(100)]
    [Column("ADMIN_NAME")]
    public string AdminName { get; set; } = string.Empty;

    [MaxLength(100)]
    [Column("EMAIL")]
    public string? Email { get; set; }

    [MaxLength(20)]
    [Column("PHONE")]
    public string? Phone { get; set; }

    [MaxLength(50)]
    [Column("ADMIN_ROLE")]
    public string AdminRole { get; set; } = "HallAdmin"; // PROVOST, ASST_PROVOST, HallAdmin

    [Column("START_DATE")]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    [Column("END_DATE")]
    public DateTime? EndDate { get; set; }

    [MaxLength(10)]
    [Column("STATUS")]
    public string Status { get; set; } = "Active"; // Active or Inactive

    [MaxLength(100)]
    [Column("REGISTRATION_TOKEN")]
    public string? RegistrationToken { get; set; } // Unique token for hall admin registration

    [Column("IS_REGISTERED")]
    public bool IsRegistered { get; set; } = false; // Whether hall admin has completed registration

    // Navigation properties
    [ForeignKey("HallId")]
    public virtual Hall Hall { get; set; } = null!;
}
