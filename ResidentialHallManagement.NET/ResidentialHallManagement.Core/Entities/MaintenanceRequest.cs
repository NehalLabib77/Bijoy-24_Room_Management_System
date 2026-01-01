using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities
{
    [Table("MAINTENANCE_REQUESTS")]
    public class MaintenanceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("REQUEST_ID")]
        public int RequestId { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("STUDENT_ID")]
        public required string StudentId { get; set; }

        [Required]
        [MaxLength(4)]
        [Column("ROOM_ID")]
        public required string RoomId { get; set; }

        [Required]
        [Column("HALL_ID")]
        public int HallId { get; set; }

        [Required]
        [MaxLength(500)]
        [Column("ISSUE")]
        public required string Issue { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("STATUS")]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Resolved, Rejected

        [Column("SUBMITTED_ON")]
        public DateTime SubmittedOn { get; set; } = DateTime.Now;

        [Column("RESOLVED_ON")]
        public DateTime? ResolvedOn { get; set; }

        [MaxLength(500)]
        [Column("TECHNICIAN_NOTE")]
        public string? TechnicianNote { get; set; }

        [MaxLength(20)]
        [Column("PRIORITY")]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("RoomId,HallId")]
        public virtual Room? Room { get; set; }
    }
}
