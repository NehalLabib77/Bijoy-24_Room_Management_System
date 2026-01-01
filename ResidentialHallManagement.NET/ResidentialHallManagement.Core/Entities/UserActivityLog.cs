using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities
{
    [Table("user_activity_log")]
    public class UserActivityLog
    {
        [Key]
        [Column("log_id")]
        public long LogId { get; set; }

        [Required]
        [Column("user_id")]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("user_type")]
        [StringLength(50)]
        public string UserType { get; set; } = string.Empty;

        [Required]
        [Column("action")]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("ip_address")]
        [StringLength(50)]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        [StringLength(500)]
        public string? UserAgent { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
