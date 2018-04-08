using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CollAction.Data
{
    public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables()
                                                                  .AddUserSecrets<Startup>()
                                                                  .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql($"Host={config["DbHost"]};Username={config["DbUser"]};Password={config["DbPassword"]};Database={config["Db"]};Port={config["DbPort"]}");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
