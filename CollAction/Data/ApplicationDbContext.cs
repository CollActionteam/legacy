using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollAction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CollAction.Services;
using Microsoft.Extensions.Options;
using CollAction.Services.Projects;
using System.Threading;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using CollAction.Services.User;

namespace CollAction.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; } = null!;

        public DbSet<ProjectCategory> ProjectCategories { get; set; } = null!;

        public DbSet<ProjectParticipant> ProjectParticipants { get; set; } = null!;

        public DbSet<ProjectTag> ProjectTags { get; set; } = null!;

        public DbSet<Tag> Tags { get; set; } = null!;

        public DbSet<ImageFile> ImageFiles { get; set; } = null!;

        public DbSet<ProjectParticipantCount> ProjectParticipantCounts { get; set; } = null!;

        public DbSet<UserEvent> UserEvents { get; set; } = null!;

        public DbSet<DonationEventLog> DonationEventLog { get; set; } = null!;

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        public static async Task InitializeDatabase(IServiceScope scope)
        {
            var provider = scope.ServiceProvider;
            var userManager = provider.GetService<UserManager<ApplicationUser>>();
            var roleManager = provider.GetService<RoleManager<IdentityRole>>();
            var context = provider.GetRequiredService<ApplicationDbContext>();
            var logger = provider.GetRequiredService<ILogger<ApplicationDbContext>>();
            var seedOptions = provider.GetRequiredService<IOptions<SeedOptions>>().Value;
            var projectService = provider.GetRequiredService<IProjectService>();
            var userService = provider.GetRequiredService<IUserService>();

            logger.LogInformation("migrating database");
            await context.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("seeding database");
            await CreateAdminRoleAndUser(seedOptions, userManager, roleManager).ConfigureAwait(false);
            await SeedTestData(seedOptions, userService, projectService).ConfigureAwait(false);
            logger.LogInformation("done starting up");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Tag>()
                   .HasAlternateKey(t => t.Name);
            builder.Entity<Project>()
                   .HasIndex(p => p.Name)
                   .HasName("IX_Projects_Name").IsUnique();
            builder.Entity<Project>()
                   .Property(p => p.DisplayPriority)
                   .HasDefaultValue(ProjectDisplayPriority.Medium);
            builder.Entity<ApplicationUser>()
                   .HasMany(p => p.Projects)
                   .WithOne(proj => proj.Owner!)
                   .HasForeignKey(proj => proj.OwnerId)
                   .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Project>()
                   .HasMany(p => p.Categories)
                   .WithOne(pc => pc.Project)
                   .HasForeignKey(pc => pc.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ProjectCategory>()
                   .HasKey("Category", "ProjectId");
            builder.Entity<ProjectParticipant>()
                   .HasKey("UserId", "ProjectId");
            builder.Entity<ProjectTag>()
                   .HasKey("TagId", "ProjectId");
            builder.Entity<Project>()
                   .HasOne(p => p.ParticipantCounts)
                   .WithOne(p => p!.Project)
                   .HasForeignKey<ProjectParticipantCount>(p => p.ProjectId);
            builder.Entity<ApplicationUser>().Property(u => u.RepresentsNumberParticipants).HasDefaultValue(1);
            builder.Entity<UserEvent>()
                   .HasOne(e => e.User)
                   .WithMany(u => u!.UserEvents)
                   .HasForeignKey(e => e.UserId);
        }

        private static async Task CreateAdminRoleAndUser(SeedOptions seedOptions, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create admin role if not exists
            IdentityRole adminRole = await roleManager.FindByNameAsync(AuthorizationConstants.AdminRole).ConfigureAwait(false);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(AuthorizationConstants.AdminRole) { NormalizedName = AuthorizationConstants.AdminRole };
                IdentityResult result = await roleManager.CreateAsync(adminRole).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error creating role.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }

            // Create admin user if not exists
            ApplicationUser admin = await userManager.FindByEmailAsync(seedOptions.AdminEmail).ConfigureAwait(false);
            if (admin == null)
            {
                admin = new ApplicationUser(userName: seedOptions.AdminEmail, email: seedOptions.AdminEmail, emailConfirmed: true, registrationDate: DateTime.UtcNow, firstName: null, lastName: null);
                IdentityResult result = await userManager.CreateAsync(admin, seedOptions.AdminPassword).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error creating user.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }

            // Assign admin role if not assigned
            if (!(await userManager.IsInRoleAsync(admin, AuthorizationConstants.AdminRole).ConfigureAwait(false)))
            {
                IdentityResult result = await userManager.AddToRoleAsync(admin, AuthorizationConstants.AdminRole).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error assigning admin role.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }
        }

        private static async Task SeedTestData(SeedOptions seedOptions, IUserService userService, IProjectService projectService)
        {
            if (seedOptions.SeedTestData && !(await projectService.SearchProjects(null, null).AnyAsync().ConfigureAwait(false)))
            {
                var seededUsers = await userService.SeedTestUsers(CancellationToken.None).ConfigureAwait(false);
                await projectService.SeedRandomProjects(seededUsers, CancellationToken.None).ConfigureAwait(false);
            }
        }
    }
}
