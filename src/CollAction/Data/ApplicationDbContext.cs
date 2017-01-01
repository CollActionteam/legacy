using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollAction.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using CollAction.Data.Geonames;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CollAction.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectParticipant> ProjectParticipants { get; set; }
        public DbSet<ProjectTag> ProjectTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationContinent> LocationContinents { get; set; }
        public DbSet<LocationCountry> LocationCountries { get; set; }
        public DbSet<LocationAlternateName> LocationAlternateNames { get; set; }
        public DbSet<LocationLevel1> LocationLevel1 { get; set; }
        public DbSet<LocationLevel2> LocationLevel2 { get; set; }

        /// <summary>
        /// Configure the model (foreign keys, relations, primary keys, etc)
        /// </summary>
        /// <param name="builder">Model builder</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Tag>().HasAlternateKey(t => t.Name);
            builder.Entity<Project>().HasAlternateKey(p => p.Name);
            builder.Entity<Project>().Property(p => p.DisplayPriority).HasDefaultValue(ProjectDisplayPriority.Medium);
            builder.Entity<ProjectParticipant>().HasKey("UserId", "ProjectId");
            builder.Entity<ProjectTag>().HasKey("TagId", "ProjectId");
            builder.Entity<Location>().HasOne(l => l.Country)
                                      .WithMany(c => c.Locations)
                                      .HasForeignKey(l => l.CountryId)
                                      .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<LocationCountry>().HasOne(c => c.Location);
            builder.Entity<Location>().HasOne(l => l.Level1)
                                      .WithMany(l => l.Locations)
                                      .HasForeignKey(l => l.Level1Id)
                                      .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<LocationLevel1>().HasOne(l => l.Location);
            builder.Entity<Location>().HasOne(l => l.Level2)
                                      .WithMany(l => l.Locations)
                                      .HasForeignKey(l => l.Level2Id)
                                      .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<LocationLevel2>().HasOne(l => l.Location);
       }

        /// <summary>
        /// Seed the database with initialisation data here
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="roleManager">Role managers to create and query roles</param>
        /// <param name="userManager">User manager to create and query users</param>
        /// <param name="token">Cancellation token</param>
        public async Task Seed(IConfigurationRoot configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            await CreateAdminRoleAndUser(configuration, userManager, roleManager);
            await CreateCategories();
            await ImportLocationData(configuration);
        }

        private async Task ImportLocationData(IConfigurationRoot configuration)
        {
            if (configuration["ImportLocationData"].Equals("1", StringComparison.Ordinal))
                await new GeonamesImporter(this).ImportLocationData();
        }

        private async Task CreateAdminRoleAndUser(IConfigurationRoot configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create admin role if not exists
            string adminRoleName = "admin";
            IdentityRole adminRole = await roleManager.FindByNameAsync(adminRoleName);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(adminRoleName) { NormalizedName = adminRoleName };
                IdentityResult result = await roleManager.CreateAsync(adminRole);
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Error creating role.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
            }

            // Create admin user if not exists
            string adminPassword = configuration["AdminPassword"];
            string adminEmail = configuration["AdminEmail"];
            ApplicationUser admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                };
                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Error creating user.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
            }

            // Assign admin role if not assigned
            if (!(await userManager.IsInRoleAsync(admin, adminRoleName)))
            {
                IdentityResult result = await userManager.AddToRoleAsync(admin, adminRoleName);
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Error assigning admin role.{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
            }

            DetachAll();
        }

        private async Task CreateCategories()
        {
            // Initialize categories
            if (!(await Categories.AnyAsync()))
            {
                Categories.AddRange(new[] {
                    new Category() { Name = "Environment", ColorHex = "FFFFFFFF", Description = "Environment", File = "" },
                    new Category() { Name = "Community", ColorHex = "FFFFFFFF", Description = "Community", File = "" },
                    new Category() { Name = "Consuming", ColorHex = "FFFFFFFF", Description = "Consuming", File = "" },
                    new Category() { Name = "Wellbeing", ColorHex = "FFFFFFFF", Description = "Wellbeing", File = "" },
                    new Category() { Name = "Governance", ColorHex = "FFFFFFFF", Description = "Governance", File = "" },
                    new Category() { Name = "Health", ColorHex = "FFFFFFFF", Description = "Health", File = "" },
                    new Category() { Name = "Other", ColorHex = "FFFFFFFF", Description = "Other", File = "" },
                });
                await SaveChangesAsync();
                DetachAll();
            }
        }

        public void DetachAll()
        {
            foreach (EntityEntry entry in ChangeTracker.Entries().ToArray())
                if (entry.Entity != null)
                    entry.State = EntityState.Detached;
        }
    }
}
