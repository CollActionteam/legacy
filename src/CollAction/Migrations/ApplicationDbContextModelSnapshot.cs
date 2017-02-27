using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CollAction.Data;

namespace CollAction.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("CollAction.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("LastName")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("CollAction.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Color");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 120);

                    b.Property<string>("File")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 40);

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("CollAction.Models.ImageFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Filepath")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 16);

                    b.Property<int>("Height");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.Property<int>("Width");

                    b.HasKey("Id");

                    b.ToTable("ImageFiles");
                });

            modelBuilder.Entity("CollAction.Models.Job", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

                    b.Property<int?>("LocationId");

                    b.Property<DateTime>("PostDate");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("CollAction.Models.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CountryId")
                        .HasAnnotation("MaxLength", 2);

                    b.Property<int>("Feature");

                    b.Property<int>("FeatureClass");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("numeric(13,10)");

                    b.Property<string>("Level1Id")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<string>("Level2Id")
                        .HasAnnotation("MaxLength", 80);

                    b.Property<string>("LocationContinentId");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("numeric(13,10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("TimeZone")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 40);

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("Level1Id");

                    b.HasIndex("Level2Id");

                    b.HasIndex("LocationContinentId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("CollAction.Models.LocationAlternateName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AlternateName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 400);

                    b.Property<bool>("IsColloquial");

                    b.Property<bool>("IsHistoric");

                    b.Property<bool>("IsPreferredName");

                    b.Property<bool>("IsShortName");

                    b.Property<string>("LanguageCode")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 7);

                    b.Property<int>("LocationId");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("LocationAlternateNames");
                });

            modelBuilder.Entity("CollAction.Models.LocationContinent", b =>
                {
                    b.Property<string>("Id")
                        .HasAnnotation("MaxLength", 2);

                    b.Property<int>("LocationId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 20);

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("LocationContinents");
                });

            modelBuilder.Entity("CollAction.Models.LocationCountry", b =>
                {
                    b.Property<string>("Id")
                        .HasAnnotation("MaxLength", 2);

                    b.Property<string>("CapitalCity")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("ContinentId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2);

                    b.Property<int>("LocationId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("ContinentId");

                    b.HasIndex("LocationId");

                    b.ToTable("LocationCountries");
                });

            modelBuilder.Entity("CollAction.Models.LocationLevel1", b =>
                {
                    b.Property<string>("Id")
                        .HasAnnotation("MaxLength", 20);

                    b.Property<int>("LocationId");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("LocationLevel1");
                });

            modelBuilder.Entity("CollAction.Models.LocationLevel2", b =>
                {
                    b.Property<string>("Id")
                        .HasAnnotation("MaxLength", 80);

                    b.Property<int>("LocationId");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("LocationLevel2");
                });

            modelBuilder.Entity("CollAction.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<string>("CreatorComments")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2048);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 1024);

                    b.Property<int?>("DescriptionVideoLinkId");

                    b.Property<int>("DisplayPriority")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(1);

                    b.Property<DateTime>("End");

                    b.Property<string>("Goal")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 1024);

                    b.Property<int?>("LocationId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("OwnerId")
                        .IsRequired();

                    b.Property<string>("Proposal")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 512);

                    b.Property<DateTime>("Start");

                    b.Property<int>("Status");

                    b.Property<int>("Target");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("DescriptionVideoLinkId");

                    b.HasIndex("LocationId");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_Projects_Name");

                    b.HasIndex("OwnerId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("CollAction.Models.ProjectParticipant", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<int>("ProjectId");

                    b.HasKey("UserId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectParticipants");
                });

            modelBuilder.Entity("CollAction.Models.ProjectTag", b =>
                {
                    b.Property<int>("TagId");

                    b.Property<int>("ProjectId");

                    b.HasKey("TagId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("TagId");

                    b.ToTable("ProjectTags");
                });

            modelBuilder.Entity("CollAction.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 120);

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("CollAction.Models.VideoLink", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Link")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("VideoLinks");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("CollAction.Models.Job", b =>
                {
                    b.HasOne("CollAction.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");
                });

            modelBuilder.Entity("CollAction.Models.Location", b =>
                {
                    b.HasOne("CollAction.Models.LocationCountry", "Country")
                        .WithMany("Locations")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CollAction.Models.LocationLevel1", "Level1")
                        .WithMany("Locations")
                        .HasForeignKey("Level1Id")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CollAction.Models.LocationLevel2", "Level2")
                        .WithMany("Locations")
                        .HasForeignKey("Level2Id")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CollAction.Models.LocationContinent")
                        .WithMany("Locations")
                        .HasForeignKey("LocationContinentId");
                });

            modelBuilder.Entity("CollAction.Models.LocationAlternateName", b =>
                {
                    b.HasOne("CollAction.Models.Location", "Location")
                        .WithMany("AlternateNames")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CollAction.Models.LocationContinent", b =>
                {
                    b.HasOne("CollAction.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CollAction.Models.LocationCountry", b =>
                {
                    b.HasOne("CollAction.Models.LocationContinent", "Continent")
                        .WithMany()
                        .HasForeignKey("ContinentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CollAction.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CollAction.Models.LocationLevel1", b =>
                {
                    b.HasOne("CollAction.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CollAction.Models.LocationLevel2", b =>
                {
                    b.HasOne("CollAction.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CollAction.Models.Project", b =>
                {
                    b.HasOne("CollAction.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CollAction.Models.VideoLink", "DescriptionVideoLink")
                        .WithMany()
                        .HasForeignKey("DescriptionVideoLinkId");

                    b.HasOne("CollAction.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("CollAction.Models.ApplicationUser", "Owner")
                        .WithMany("Projects")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CollAction.Models.ProjectParticipant", b =>
                {
                    b.HasOne("CollAction.Models.Project", "Project")
                        .WithMany("Participants")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CollAction.Models.ApplicationUser", "User")
                        .WithMany("Participates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CollAction.Models.ProjectTag", b =>
                {
                    b.HasOne("CollAction.Models.Project", "Project")
                        .WithMany("Tags")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CollAction.Models.Tag", "Tag")
                        .WithMany("ProjectTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CollAction.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CollAction.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CollAction.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
