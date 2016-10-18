using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollAction.Models;

namespace CollAction.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Project { get; set; }

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
    }
}
