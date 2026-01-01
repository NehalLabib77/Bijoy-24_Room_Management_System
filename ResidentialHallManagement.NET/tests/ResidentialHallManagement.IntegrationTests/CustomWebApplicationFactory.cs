using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResidentialHallManagement.Data;

namespace ResidentialHallManagement.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registrations
            var descriptor1 = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<HallManagementDbContext>));
            if (descriptor1 != null)
                services.Remove(descriptor1);

            var descriptor2 = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor2 != null)
                services.Remove(descriptor2);

            // Add in-memory databases for testing
            services.AddDbContext<HallManagementDbContext>(options =>
                options.UseInMemoryDatabase("Test_HallDb"));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("Test_IdentityDb"));

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Ensure databases are created
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var hallDb = scopedServices.GetRequiredService<HallManagementDbContext>();
                hallDb.Database.EnsureCreated();

                var identityDb = scopedServices.GetRequiredService<ApplicationDbContext>();
                identityDb.Database.EnsureCreated();
            }
        });

        return base.CreateHost(builder);
    }
}
