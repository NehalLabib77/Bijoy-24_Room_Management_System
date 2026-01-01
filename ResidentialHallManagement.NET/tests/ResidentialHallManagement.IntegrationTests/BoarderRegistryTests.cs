using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ResidentialHallManagement.Data;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Web.ViewModels;
using ResidentialHallManagement.Web.Controllers;
using Microsoft.AspNetCore.Identity;

namespace ResidentialHallManagement.IntegrationTests;

public class BoarderRegistryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BoarderRegistryTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterStudent_WithValidBoarderAssignsRegistryAndCreatesStudent()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<HallManagementDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();

        // Seed a boarder
        db.BoarderRegistries.Add(new BoarderRegistry { BoarderNo = "BTST001", Name = "TestBoarder", Status = "Available" });
        await db.SaveChangesAsync();

        var controller = new AccountController(signInManager, userManager, db);
        var model = new RegisterViewModel
        {
            Username = "testuser",
            Email = "test@example.com",
            UserType = "Student",
            StudentId = "S20251210",
            FullName = "Test Student",
            Password = "Password1",
            ConfirmPassword = "Password1",
            BoarderNo = "BTST001"
        };

        var result = await controller.Register(model);

        var student = await db.Students.FindAsync("S20251210");
        Assert.NotNull(student);
        Assert.Equal("BTST001", student.BoarderNo);

        var registry = await db.BoarderRegistries.FindAsync("BTST001");
        Assert.NotNull(registry);
        Assert.Equal("Assigned", registry.Status);
        Assert.Equal("S20251210", registry.StudentId);
    }
}
