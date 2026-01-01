using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities
{
    [Table("hall_admins_info")]
    public class HallAdminInfo
    {
        [Key]
        [Column("hall_admin_id")]
        public int HallAdminId { get; set; }

        [Required]
        [Column("user_id")]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("hall_id")]
        public int HallId { get; set; }

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

        [Column("admin_role")]
        [StringLength(50)]
        public string AdminRole { get; set; } = "HallAdmin";

        [Column("employee_id")]
        [StringLength(50)]
        public string? EmployeeId { get; set; }

        [Column("designation")]
        [StringLength(100)]
        public string? Designation { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("registration_token")]
        [StringLength(100)]
        public string? RegistrationToken { get; set; }

        [Column("is_registered")]
        public bool IsRegistered { get; set; } = false;

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

        [Column("address")]
        public string? Address { get; set; }

        [Column("emergency_contact")]
        [StringLength(100)]
        public string? EmergencyContact { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("HallId")]
        public virtual Hall? Hall { get; set; }
    }
}
