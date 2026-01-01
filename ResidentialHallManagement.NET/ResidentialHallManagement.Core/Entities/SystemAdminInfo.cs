using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities
{
    [Table("system_admins")]
    public class SystemAdminInfo
    {
        [Key]
        [Column("admin_id")]
        public int AdminId { get; set; }

        [Required]
        [Column("user_id")]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("username")]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("full_name")]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Column("phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("role")]
        [StringLength(50)]
        public string Role { get; set; } = "SystemAdmin";

        [Column("department")]
        [StringLength(100)]
        public string? Department { get; set; }

        [Column("employee_id")]
        [StringLength(50)]
        public string? EmployeeId { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_super_admin")]
        public bool IsSuperAdmin { get; set; } = false;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("created_by")]
        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Column("updated_by")]
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Column("profile_image_url")]
        [StringLength(500)]
        public string? ProfileImageUrl { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
