using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities
{
    [Table("students_info")]
    public class StudentInfo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("student_id")]
        [StringLength(50)]
        public string StudentId { get; set; } = string.Empty;

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

        [Column("father_name")]
        [StringLength(200)]
        public string? FatherName { get; set; }

        [Column("mother_name")]
        [StringLength(200)]
        public string? MotherName { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("gender")]
        [StringLength(20)]
        public string? Gender { get; set; }

        [Column("blood_group")]
        [StringLength(10)]
        public string? BloodGroup { get; set; }

        [Column("religion")]
        [StringLength(50)]
        public string? Religion { get; set; }

        [Column("nationality")]
        [StringLength(50)]
        public string Nationality { get; set; } = "Bangladeshi";

        [Column("phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("emergency_contact")]
        [StringLength(100)]
        public string? EmergencyContact { get; set; }

        [Column("emergency_contact_name")]
        [StringLength(200)]
        public string? EmergencyContactName { get; set; }

        [Column("emergency_contact_relation")]
        [StringLength(50)]
        public string? EmergencyContactRelation { get; set; }

        [Column("permanent_address")]
        public string? PermanentAddress { get; set; }

        [Column("present_address")]
        public string? PresentAddress { get; set; }

        [Column("boarder_no")]
        [StringLength(50)]
        public string? BoarderNo { get; set; }

        [Required]
        [Column("faculty")]
        [StringLength(100)]
        public string Faculty { get; set; } = string.Empty;

        [Column("department")]
        [StringLength(100)]
        public string? Department { get; set; }

        [Required]
        [Column("semester")]
        public int Semester { get; set; }

        [Column("session")]
        [StringLength(20)]
        public string? Session { get; set; }

        [Column("cgpa")]
        public decimal? CGPA { get; set; }

        [Column("admission_date")]
        public DateTime? AdmissionDate { get; set; }

        [Column("hall_id")]
        public int? HallId { get; set; }

        [Column("room_id")]
        public int? RoomId { get; set; }

        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; } = "RUNNING";

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("last_login")]
        public DateTime? LastLogin { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Column("profile_image_url")]
        [StringLength(500)]
        public string? ProfileImageUrl { get; set; }

        [Column("guardian_info")]
        public string? GuardianInfo { get; set; }

        [Column("medical_info")]
        public string? MedicalInfo { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("HallId")]
        public virtual Hall? Hall { get; set; }

        // Note: Room relationship commented out due to composite key conflict
        // [ForeignKey("RoomId")]
        // public virtual Room? Room { get; set; }
    }
}
