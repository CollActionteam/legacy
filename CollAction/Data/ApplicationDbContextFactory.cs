using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CollAction.Data
{
    /// <summary>
    /// Context factory (only for use in migrations, with dummy connection string)
    /// </summary>
    public class ApplicationDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext Create(DbContextFactoryOptions options)
        {
            DbContextOptionsBuilder<ApplicationDbContext> dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>();
            dbOptions.UseNpgsql($"Host=DbHost;Username=DbUser;Password=DbPassword;Database=Db;Port=1");
            return new ApplicationDbContext(dbOptions.Options);
        }
    }
}
