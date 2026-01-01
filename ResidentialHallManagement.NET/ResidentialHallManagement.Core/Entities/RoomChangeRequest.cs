using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities
{
    [Table("ROOM_CHANGE_REQUESTS")]
    public class RoomChangeRequest
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
        [Column("REQUESTED_ROOM_ID")]
        public required string RequestedRoomId { get; set; }

        [Required]
        [Column("REQUESTED_HALL_ID")]
        public int RequestedHallId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("STATUS")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Column("REQUEST_DATE")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Column("PROCESSED_DATE")]
        public DateTime? ProcessedDate { get; set; }

        [MaxLength(500)]
        [Column("REASON")]
        public string? Reason { get; set; }

        [MaxLength(500)]
        [Column("ADMIN_REMARKS")]
        public string? AdminRemarks { get; set; }

        // Navigation properties
        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        [ForeignKey("RequestedRoomId,RequestedHallId")]
        public Room? RequestedRoom { get; set; }
    }
}
