using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollAction.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace CollAction.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Project { get; set; }
        public DbSet<Subscription> Subscription { get; set; }

        /// <summary>
        /// Configure the model (foreign keys, relations, primary keys, etc)
        /// </summary>
        /// <param name="builder">Model builder</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                   .HasOne(p => p.Owner)
                   .WithMany(u => u.Projects)
                   .HasForeignKey(p => p.OwnerId);

            builder.Entity<Project>()
                   .HasMany(p => p.Subscriptions)
                   .WithOne(s => s.Project)
                   .HasForeignKey(s => s.ProjectId);

            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.Subscriptions)
                   .WithOne(s => s.User)
                   .HasForeignKey(s => s.UserId);
        }

        /// <summary>
        /// Seed the database with initialisation data here
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <param name="token">Cancellation token</param>
        public async Task Seed(IConfigurationRoot configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create admin role if not exists
            string adminRoleName = "admin";
            IdentityRole adminRole = await roleManager.FindByNameAsync(adminRoleName);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(adminRoleName);
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
        }
    }
}
