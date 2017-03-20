using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace CollAction.Data
{
    /// <summary>
    /// Context factory (only for use in migrations
    /// </summary>
    public class ApplicationDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext Create(DbContextFactoryOptions options)
        {
            var factoryConfig = new ConfigurationBuilder().AddUserSecrets<ApplicationDbContextFactory>().Build();
            DbContextOptionsBuilder<ApplicationDbContext> dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbOptions.UseNpgsql($"Host={factoryConfig["DbHost"]};Username={factoryConfig["DbUser"]};Password={factoryConfig["DbPassword"]};Database={factoryConfig["Db"]};Port={factoryConfig["DbPort"]}");
            return new ApplicationDbContext(dbOptions.Options);
        }
    }
}
