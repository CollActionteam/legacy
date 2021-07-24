using CollAction.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace CollAction.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Crowdaction> Crowdactions { get; set; } = null!;

        public DbSet<CrowdactionCategory> CrowdactionCategories { get; set; } = null!;

        public DbSet<CrowdactionParticipant> CrowdactionParticipants { get; set; } = null!;

        public DbSet<CrowdactionTag> CrowdactionTags { get; set; } = null!;

        public DbSet<CrowdactionComment> CrowdactionComments { get; set; } = null!;

        public DbSet<Tag> Tags { get; set; } = null!;

        public DbSet<ImageFile> ImageFiles { get; set; } = null!;

        public DbSet<CrowdactionParticipantCount> CrowdactionParticipantCounts { get; set; } = null!;

        public DbSet<UserEvent> UserEvents { get; set; } = null!;

        public DbSet<DonationEventLog> DonationEventLog { get; set; } = null!;

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Tag>()
                   .HasAlternateKey(t => t.Name);
            builder.Entity<Crowdaction>()
                   .HasIndex(c => c.Name)
                   .HasDatabaseName("IX_Crowdactions_Name").IsUnique();
            builder.Entity<Crowdaction>()
                   .HasMany(c => c.Comments)
                   .WithOne(c => c.Crowdaction!)
                   .HasForeignKey(c => c.CrowdactionId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Crowdaction>()
                   .Property(c => c.DisplayPriority)
                   .HasDefaultValue(CrowdactionDisplayPriority.Medium);
            builder.Entity<ApplicationUser>()
                   .HasMany(c => c.Crowdactions)
                   .WithOne(proj => proj.Owner!)
                   .HasForeignKey(proj => proj.OwnerId)
                   .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<ApplicationUser>()
                   .HasMany(a => a.CrowdactionComments)
                   .WithOne(c => c.User!)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Crowdaction>()
                   .HasMany(c => c.Categories)
                   .WithOne(pc => pc.Crowdaction!)
                   .HasForeignKey(pc => pc.CrowdactionId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CrowdactionCategory>()
                   .HasKey("Category", "CrowdactionId");
            builder.Entity<CrowdactionParticipant>()
                   .HasKey("UserId", "CrowdactionId");
            builder.Entity<CrowdactionTag>()
                   .HasKey("TagId", "CrowdactionId");
            builder.Entity<Crowdaction>()
                   .HasOne(c => c.ParticipantCounts)
                   .WithOne(c => c!.Crowdaction!)
                   .HasForeignKey<CrowdactionParticipantCount>(c => c.CrowdactionId);
            builder.Entity<ApplicationUser>().Property(u => u.RepresentsNumberParticipants).HasDefaultValue(1);
            builder.Entity<UserEvent>()
                   .HasOne(e => e.User)
                   .WithMany(u => u!.UserEvents)
                   .HasForeignKey(e => e.UserId);

            // All stored dates are UTC
            ValueConverter<DateTime, DateTime> dateTimeConverter =
                new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

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
