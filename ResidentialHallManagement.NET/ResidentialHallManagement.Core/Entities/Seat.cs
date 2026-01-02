using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResidentialHallManagement.Core.Entities;

/// <summary>
/// Seat type enumeration for room positions
/// </summary>
public static class SeatTypes
{
    public const string WINDOW_LEFT = "WINDOW_LEFT";
    public const string WINDOW_RIGHT = "WINDOW_RIGHT";
    public const string DOOR_LEFT = "DOOR_LEFT";
    public const string DOOR_RIGHT = "DOOR_RIGHT";

    public static string GetDisplayName(string seatType)
    {
        return seatType switch
        {
            WINDOW_LEFT => "Window Side - Left",
            WINDOW_RIGHT => "Window Side - Right",
            DOOR_LEFT => "Door Side - Left",
            DOOR_RIGHT => "Door Side - Right",
            _ => seatType
        };
    }

    public static bool IsWindowSide(string seatType) => seatType?.StartsWith("WINDOW") ?? false;
    public static bool IsDoorSide(string seatType) => seatType?.StartsWith("DOOR") ?? false;
    public static bool IsLeftPosition(string seatType) => seatType?.EndsWith("LEFT") ?? false;
    public static bool IsRightPosition(string seatType) => seatType?.EndsWith("RIGHT") ?? false;
}

/// <summary>
/// Seat status enumeration
/// </summary>
public static class SeatStatuses
{
    public const string Available = "Available";
    public const string Pending = "Pending";
    public const string Booked = "Booked";
    public const string Reserved = "Reserved";
    public const string Maintenance = "Maintenance";
}

/// <summary>
/// Represents an individual seat/bed within a room.
/// Each room typically has 4 seats: 2 window-side and 2 door-side.
/// </summary>
public class Seat
{
    [Key]
    [Column("SEAT_ID")]
    public int SeatId { get; set; }

    [Required]
    [Column("ROOM_ID")]
    [MaxLength(4)]
    public string RoomId { get; set; } = string.Empty;

    [Required]
    [Column("HALL_ID")]
    public int HallId { get; set; }

    [Required]
    [Column("SEAT_NUMBER")]
    public int SeatNumber { get; set; } // 1, 2, 3, 4

    [Required]
    [MaxLength(20)]
    [Column("SEAT_TYPE")]
    public string SeatType { get; set; } = SeatTypes.WINDOW_LEFT; // WINDOW_LEFT, WINDOW_RIGHT, DOOR_LEFT, DOOR_RIGHT

    [Required]
    [MaxLength(20)]
    [Column("STATUS")]
    public string Status { get; set; } = SeatStatuses.Available; // Available, Pending, Booked, Reserved, Maintenance

    [MaxLength(10)]
    [Column("BOOKED_BY_STUDENT_ID")]
    public string? BookedByStudentId { get; set; }

    [Column("BOOKED_ON")]
    public DateTime? BookedOn { get; set; }

    [MaxLength(50)]
    [Column("SEAT_LABEL")]
    public string? SeatLabel { get; set; } // e.g., "Window Side - Left", "Door Side - Right"

    [MaxLength(10)]
    [Column("POSITION")]
    public string? Position { get; set; } // LEFT or RIGHT

    [Column("IS_TEMPORARILY_HELD")]
    public bool IsTemporarilyHeld { get; set; } = false;

    [MaxLength(10)]
    [Column("HELD_BY_STUDENT_ID")]
    public string? HeldByStudentId { get; set; }

    [Column("HELD_UNTIL")]
    public DateTime? HeldUntil { get; set; }

    [Column("CREATED_AT")]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("UPDATED_AT")]
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("RoomId,HallId")]
    public virtual Room? Room { get; set; }

    [ForeignKey("BookedByStudentId")]
    public virtual Student? BookedByStudent { get; set; }

    [ForeignKey("HeldByStudentId")]
    public virtual Student? HeldByStudent { get; set; }

    // Helper properties for UI
    [NotMapped]
    public bool IsWindowSide => SeatTypes.IsWindowSide(SeatType);

    [NotMapped]
    public bool IsDoorSide => SeatTypes.IsDoorSide(SeatType);

    [NotMapped]
    public bool IsLeftPosition => SeatTypes.IsLeftPosition(SeatType);

    [NotMapped]
    public bool IsRightPosition => SeatTypes.IsRightPosition(SeatType);

    [NotMapped]
    public bool IsAvailable => Status == SeatStatuses.Available && !IsTemporarilyHeld;

    [NotMapped]
    public bool IsBooked => Status == SeatStatuses.Booked;

    [NotMapped]
    public bool IsPending => Status == SeatStatuses.Pending || Status == SeatStatuses.Reserved;

    [NotMapped]
    public string DisplaySeatType => SeatTypes.GetDisplayName(SeatType);
}
