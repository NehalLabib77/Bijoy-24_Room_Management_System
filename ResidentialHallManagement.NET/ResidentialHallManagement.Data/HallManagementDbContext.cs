using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;

namespace ResidentialHallManagement.Data;

public class HallManagementDbContext : DbContext
{
    public HallManagementDbContext(DbContextOptions<HallManagementDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for all entities
    public DbSet<Hall> Halls { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<HallAdmin> HallAdmins { get; set; }
    public DbSet<AdminUser> AdminUsers { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<AssignedHall> AssignedHalls { get; set; }
    public DbSet<AssignedRoom> AssignedRooms { get; set; }
    public DbSet<RoomChangeRequest> RoomChangeRequests { get; set; }
    public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
    public DbSet<RoomAssignment> RoomAssignments { get; set; }
    public DbSet<RoomApplicationRequest> RoomApplicationRequests { get; set; }
    public DbSet<BoarderRegistry> BoarderRegistries { get; set; }

    // Seat Management Tables
    public DbSet<Seat> Seats { get; set; }
    public DbSet<SeatApplication> SeatApplications { get; set; }

    // New User Information Tables
    public DbSet<SystemAdminInfo> SystemAdminsInfo { get; set; }
    public DbSet<HallAdminInfo> HallAdminsInfo { get; set; }
    public DbSet<StudentInfo> StudentsInfo { get; set; }
    public DbSet<UserActivityLog> UserActivityLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names to match MySQL schema (lowercase on Windows)
        modelBuilder.Entity<Hall>().ToTable("hall");
        modelBuilder.Entity<Student>().ToTable("students");
        modelBuilder.Entity<Admin>().ToTable("admins");
        modelBuilder.Entity<HallAdmin>().ToTable("hall_admins");
        modelBuilder.Entity<AdminUser>().ToTable("admin_users");
        modelBuilder.Entity<Room>().ToTable("rooms");
        modelBuilder.Entity<BoarderRegistry>().ToTable("boarder_registry");
        modelBuilder.Entity<AssignedHall>().ToTable("assigned_hall");
        modelBuilder.Entity<AssignedRoom>().ToTable("assigned_room");
        modelBuilder.Entity<RoomAssignment>().ToTable("room_assignments");
        modelBuilder.Entity<RoomApplicationRequest>().ToTable("room_application_requests");

        // Seat Management Tables
        modelBuilder.Entity<Seat>().ToTable("seats");
        modelBuilder.Entity<SeatApplication>().ToTable("seat_applications");

        // Configure composite keys
        modelBuilder.Entity<Room>()
            .HasKey(r => new { r.RoomId, r.HallId });

        modelBuilder.Entity<AssignedHall>()
            .HasKey(ah => new { ah.StudentId, ah.HallId });

        // Configure relationships
        modelBuilder.Entity<AssignedRoom>()
            .HasOne(ar => ar.Room)
            .WithMany(r => r.AssignedRooms)
            .HasForeignKey(ar => new { ar.RoomId, ar.AssignedHallId })
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AssignedRoom>()
            .HasOne(ar => ar.AssignedHall)
            .WithMany(ah => ah.AssignedRooms)
            .HasForeignKey(ar => new { ar.StudentId, ar.AssignedHallId })
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Student to BoarderRegistry relationship
        // Student must have a BoarderNo from BoarderRegistry
        modelBuilder.Entity<Student>()
            .HasOne(s => s.BoarderRegistry)
            .WithMany()
            .HasForeignKey(s => s.BoarderNo)
            .HasPrincipalKey(br => br.BoarderNo)
            .OnDelete(DeleteBehavior.Restrict);

        // Note: BoarderRegistry.StudentId does NOT have a FK constraint
        // This allows system admin to create registry entries with student IDs
        // that don't exist yet (pre-registration/allocation)

        // Configure check constraints and defaults
        modelBuilder.Entity<AssignedHall>()
            .Property(ah => ah.IsCurrent)
            .HasDefaultValue("YES");

        // Add seed data for login credentials (you can customize this)
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Add initial admin user data here if needed
        // For example, you might want to seed a default admin hall or user
    }
}
