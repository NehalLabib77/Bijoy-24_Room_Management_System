using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

public class BoarderRegistry
{
    [Key]
    [MaxLength(20)]
    [Column("BOARDER_NO")]
    public string BoarderNo { get; set; } = string.Empty;

    [MaxLength(50)]
    [Column("NAME")]
    public string? Name { get; set; }

    [MaxLength(10)]
    [Column("STATUS")]
    public string Status { get; set; } = "Available"; // Available or Assigned

    [MaxLength(10)]
    [Column("STUDENT_ID")]
    public string? StudentId { get; set; }

    // Navigation property (optional pre-assignment to a student)
    public virtual Student? Student { get; set; }
}
