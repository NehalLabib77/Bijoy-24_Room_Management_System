using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

[Table("admin_users")]
public class AdminUser
{
    [Key]
    [Column("ADMIN_USER_ID")]
    public int AdminUserId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("USERNAME")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [Column("PASSWORD")]
    public string Password { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("ROLE")]
    public string Role { get; set; } = "SystemAdmin"; // SystemAdmin or HallAdmin

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("IS_ACTIVE")]
    public bool IsActive { get; set; } = true;
}
