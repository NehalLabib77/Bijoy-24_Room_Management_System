using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class Admin
{
    [Key]
    [Column("ADMIN_ID")]
    public int AdminId { get; set; }

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
    [Column("ROLE")]
    public string Role { get; set; } = "SuperAdmin"; // SuperAdmin, SystemAdmin

    [Column("CREATED_DATE")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Column("MODIFIED_DATE")]
    public DateTime? ModifiedDate { get; set; }

    [MaxLength(10)]
    [Column("STATUS")]
    public string Status { get; set; } = "Active"; // Active or Inactive
}
