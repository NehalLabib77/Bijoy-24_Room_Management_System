using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ResidentialHallManagement.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<HallManagementDbContext>
    {
        public HallManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HallManagementDbContext>();

            // SAME CONNECTION STRING FROM appsettings.json
            var connectionString = "Server=localhost;Port=3306;Database=hallmanagementdb;User=root;Password=;";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new HallManagementDbContext(optionsBuilder.Options);
        }
    }
}
