using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ResidentialHallManagement.Data;
using ResidentialHallManagement.Core.Entities;
using Xunit;

namespace ResidentialHallManagement.IntegrationTests
{
    public class RoomAssignmentTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public RoomAssignmentTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AssignRoom_DecreasesAvailableSlots()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HallManagementDbContext>();

            // Seed test data
            var hall = new Hall
            {
                HallId = 999,
                HallName = "Test Hall",
                HallCapacity = 100,
                HallType = "Male",
                Location = "Test Location"
            };
            context.Halls.Add(hall);

            var room = new Room
            {
                RoomId = 999,
                RoomNo = "T-999",
                RoomCapacity = 3,
                AvailableSlots = 3,
                RoomType = "Standard",
                Status = "Available",
                HallId = 999
            };
            context.Rooms.Add(room);

            var student = new Student
            {
                StudentId = "TEST999",
                Name = "Test Student",
                Email = "teststudent@example.com",
                PhoneNumber = "1234567890",
                Address = "Test Address",
                BoarderNo = "B999",
                DateOfBirth = DateTime.Parse("2000-01-01")
            };
            context.Students.Add(student);

            await context.SaveChangesAsync();

            var roomManagementService = scope.ServiceProvider.GetRequiredService<ResidentialHallManagement.Data.Services.IRoomManagementService>();

            // Act
            var assignment = await roomManagementService.AssignRoomAsync("TEST999", 999, "T-999");

            // Assert
            Assert.NotNull(assignment);

            var updatedRoom = await context.Rooms.FindAsync(999);
            Assert.Equal(2, updatedRoom.AvailableSlots); // Decreased from 3 to 2
            Assert.Equal("Available", updatedRoom.Status); // Still Available since capacity > 0
        }

        [Fact]
        public async Task AssignRoom_SetsStatusToOccupied_WhenFull()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HallManagementDbContext>();

            var hall = new Hall
            {
                HallId = 998,
                HallName = "Test Hall 2",
                HallCapacity = 100,
                HallType = "Male",
                Location = "Test Location"
            };
            context.Halls.Add(hall);

            var room = new Room
            {
                RoomId = 998,
                RoomNo = "T-998",
                RoomCapacity = 1,
                AvailableSlots = 1,
                RoomType = "Standard",
                Status = "Available",
                HallId = 998
            };
            context.Rooms.Add(room);

            var student = new Student
            {
                StudentId = "TEST998",
                Name = "Test Student 2",
                Email = "teststudent2@example.com",
                PhoneNumber = "1234567891",
                Address = "Test Address",
                BoarderNo = "B998",
                DateOfBirth = DateTime.Parse("2000-01-01")
            };
            context.Students.Add(student);

            await context.SaveChangesAsync();

            var roomManagementService = scope.ServiceProvider.GetRequiredService<ResidentialHallManagement.Data.Services.IRoomManagementService>();

            // Act
            var assignment = await roomManagementService.AssignRoomAsync("TEST998", 998, "T-998");

            // Assert
            Assert.NotNull(assignment);

            var updatedRoom = await context.Rooms.FindAsync(998);
            Assert.Equal(0, updatedRoom.AvailableSlots); // Decreased to 0
            Assert.Equal("Occupied", updatedRoom.Status); // Status changed to Occupied
        }

        [Fact]
        public async Task RemoveRoomAssignment_IncreasesAvailableSlots()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HallManagementDbContext>();

            var hall = new Hall
            {
                HallId = 997,
                HallName = "Test Hall 3",
                HallCapacity = 100,
                HallType = "Male",
                Location = "Test Location"
            };
            context.Halls.Add(hall);

            var room = new Room
            {
                RoomId = 997,
                RoomNo = "T-997",
                RoomCapacity = 3,
                AvailableSlots = 1, // Already 2 students assigned
                RoomType = "Standard",
                Status = "Available",
                HallId = 997
            };
            context.Rooms.Add(room);

            var student = new Student
            {
                StudentId = "TEST997",
                Name = "Test Student 3",
                Email = "teststudent3@example.com",
                PhoneNumber = "1234567892",
                Address = "Test Address",
                BoarderNo = "B997",
                DateOfBirth = DateTime.Parse("2000-01-01")
            };
            context.Students.Add(student);

            var assignment = new AssignedRoom
            {
                StudentId = "TEST997",
                HallId = 997,
                RoomId = 997,
                StartDate = DateTime.Now.AddDays(-30),
                Status = "Active"
            };
            context.AssignedRooms.Add(assignment);

            await context.SaveChangesAsync();

            var roomManagementService = scope.ServiceProvider.GetRequiredService<ResidentialHallManagement.Data.Services.IRoomManagementService>();

            // Act
            var result = await roomManagementService.RemoveRoomAssignmentAsync("TEST997");

            // Assert
            Assert.True(result);

            var updatedRoom = await context.Rooms.FindAsync(997);
            Assert.Equal(2, updatedRoom.AvailableSlots); // Increased from 1 to 2
        }

        [Fact]
        public async Task ApplyForRoom_CreatesRoomApplicationRequest()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HallManagementDbContext>();

            var hall = new Hall
            {
                HallId = 996,
                HallName = "Test Hall 4",
                HallCapacity = 100,
                HallType = "Male",
                Location = "Test Location"
            };
            context.Halls.Add(hall);

            var room = new Room
            {
                RoomId = 996,
                RoomNo = "T-996",
                RoomCapacity = 3,
                AvailableSlots = 3,
                RoomType = "Standard",
                Status = "Available",
                HallId = 996
            };
            context.Rooms.Add(room);

            var student = new Student
            {
                StudentId = "TEST996",
                Name = "Test Student 4",
                Email = "teststudent4@example.com",
                PhoneNumber = "1234567893",
                Address = "Test Address",
                BoarderNo = "B996",
                DateOfBirth = DateTime.Parse("2000-01-01")
            };
            context.Students.Add(student);

            await context.SaveChangesAsync();

            var roomManagementService = scope.ServiceProvider.GetRequiredService<ResidentialHallManagement.Data.Services.IRoomManagementService>();

            // Act
            var application = await roomManagementService.CreateApplicationAsync("TEST996", 996, "T-996");

            // Assert
            Assert.NotNull(application);
            Assert.Equal("TEST996", application.StudentId);
            Assert.Equal(996, application.HallId);
            Assert.Equal("T-996", application.RequestedRoomIdentity);
            Assert.Equal("Pending", application.Status);

            // Verify it's persisted
            var savedApplication = await context.RoomApplicationRequests
                .FirstOrDefaultAsync(r => r.StudentId == "TEST996");
            Assert.NotNull(savedApplication);
        }
    }
}
