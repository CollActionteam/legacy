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
using System.Collections.Generic;

namespace CollAction.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
            var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            var seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;

            logger.LogInformation("migrating database");
            await context.Database.MigrateAsync();
            logger.LogInformation("seeding database");
            await context.Seed(seedOptions, userManager, roleManager);
            logger.LogInformation("done starting up");
        }

        public async Task Seed(SeedOptions seedOptions, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await CreateAdminRoleAndUser(seedOptions, userManager, roleManager);
            await SeedTestProjects(seedOptions, userManager);
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
                   .WithOne(proj => proj.Owner)
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
                   .WithOne(p => p.Project)
                   .HasForeignKey<ProjectParticipantCount>(p => p.ProjectId);
            builder.Entity<ApplicationUser>().Property(u => u.RepresentsNumberParticipants).HasDefaultValue(1);
            builder.Entity<UserEvent>()
                   .HasOne(e => e.User)
                   .WithMany(u => u.UserEvents)
                   .HasForeignKey(e => e.UserId);
        }

        private async Task CreateAdminRoleAndUser(SeedOptions seedOptions, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create admin role if not exists
            IdentityRole adminRole = await roleManager.FindByNameAsync(Constants.AdminRole);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(Constants.AdminRole) { NormalizedName = Constants.AdminRole };
                IdentityResult result = await roleManager.CreateAsync(adminRole);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error creating role.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }

            // Create admin user if not exists
            ApplicationUser admin = await userManager.FindByEmailAsync(seedOptions.AdminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser(userName: seedOptions.AdminEmail, email: seedOptions.AdminEmail, emailConfirmed: true, registrationDate: DateTime.UtcNow, firstName: null, lastName: null);
                IdentityResult result = await userManager.CreateAsync(admin, seedOptions.AdminPassword);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error creating user.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }

            // Assign admin role if not assigned
            if (!(await userManager.IsInRoleAsync(admin, Constants.AdminRole)))
            {
                IdentityResult result = await userManager.AddToRoleAsync(admin, Constants.AdminRole);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Error assigning admin role.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
                }
            }
        }

        private async Task SeedTestProjects(SeedOptions seedOptions, UserManager<ApplicationUser> userManager)
        {
            if (seedOptions.SeedTestProjects && !(await Projects.AnyAsync()))
            {
                Random r = new Random();
                ApplicationUser admin = await userManager.FindByEmailAsync(seedOptions.AdminEmail);
                Projects.AddRange(
                    Enumerable.Range(0, r.Next(20, 200))
                              .Select(i =>
                                  new Project(
                                      name: Guid.NewGuid().ToString(),
                                      description: Guid.NewGuid().ToString(),
                                      start: DateTime.Now.AddDays(r.Next(-10, 10)),
                                      end: DateTime.Now.AddDays(r.Next(20, 30)),
                                      categories: new List<ProjectCategory>() { new ProjectCategory((Category)r.Next(2)), new ProjectCategory((Category)(r.Next(3) + 2)) },
                                      tags: new List<ProjectTag>(),
                                      creatorComments: Guid.NewGuid().ToString(),
                                      displayPriority: (ProjectDisplayPriority)r.Next(0, 2),
                                      goal: Guid.NewGuid().ToString(),
                                      ownerId: admin.Id,
                                      proposal: Guid.NewGuid().ToString(),
                                      status: (ProjectStatus)r.Next(0, 3),
                                      target: r.Next(1, 100000),
                                      descriptionVideoLink: Guid.NewGuid().ToString())));
                await SaveChangesAsync();
            }
        }
    }
}
