using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollAction.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using Microsoft.EntityFrameworkCore.Metadata;

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

            // All stored dates are UTC
            ValueConverter<DateTime, DateTime> dateTimeConverter = 
                new ValueConverter<DateTime, DateTime>(
                    v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            foreach (IMutableEntityType entityType in builder.Model.GetEntityTypes())
            {
                foreach (IMutableProperty property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                }
            }
        }
    }
}
