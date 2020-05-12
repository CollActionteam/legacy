﻿// <auto-generated />
using System;
using CollAction.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CollAction.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200512224012_CrowdactionComments")]
    partial class CrowdactionComments
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("CollAction.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .HasColumnType("character varying(250)")
                        .HasMaxLength(250);

                    b.Property<string>("LastName")
                        .HasColumnType("character varying(250)")
                        .HasMaxLength(250);

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("RepresentsNumberParticipants")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1);

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("CollAction.Models.Crowdaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AnonymousUserParticipants")
                        .HasColumnType("integer");

                    b.Property<int?>("BannerImageFileId")
                        .HasColumnType("integer");

                    b.Property<int?>("CardImageFileId")
                        .HasColumnType("integer");

                    b.Property<string>("CreatorComments")
                        .HasColumnType("character varying(20000)")
                        .HasMaxLength(20000);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("character varying(10000)")
                        .HasMaxLength(10000);

                    b.Property<string>("DescriptionVideoLink")
                        .HasColumnType("text");

                    b.Property<int?>("DescriptiveImageFileId")
                        .HasColumnType("integer");

                    b.Property<int>("DisplayPriority")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(1);

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FinishJobId")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Goal")
                        .IsRequired()
                        .HasColumnType("character varying(10000)")
                        .HasMaxLength(10000);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(50)")
                        .HasMaxLength(50);

                    b.Property<int>("NumberCrowdactionEmailsSent")
                        .HasColumnType("integer");

                    b.Property<string>("OwnerId")
                        .HasColumnType("text");

                    b.Property<string>("Proposal")
                        .IsRequired()
                        .HasColumnType("character varying(300)")
                        .HasMaxLength(300);

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("Target")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BannerImageFileId");

                    b.HasIndex("CardImageFileId");

                    b.HasIndex("DescriptiveImageFileId");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_Crowdactions_Name");

                    b.HasIndex("OwnerId");

                    b.ToTable("Crowdactions");
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionCategory", b =>
                {
                    b.Property<int>("Category")
                        .HasColumnType("integer");

                    b.Property<int>("CrowdactionId")
                        .HasColumnType("integer");

                    b.HasKey("Category", "CrowdactionId");

                    b.HasIndex("CrowdactionId");

                    b.ToTable("CrowdactionCategories");
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CommentedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("CrowdactionId")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CrowdactionId");

                    b.HasIndex("UserId");

                    b.ToTable("CrowdactionComments");
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionParticipant", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<int>("CrowdactionId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ParticipationDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("SubscribedToCrowdactionEmails")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UnsubscribeToken")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "CrowdactionId");

                    b.HasIndex("CrowdactionId");

                    b.ToTable("CrowdactionParticipants");
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionParticipantCount", b =>
                {
                    b.Property<int>("CrowdactionId")
                        .HasColumnType("integer");

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.HasKey("CrowdactionId");

                    b.ToTable("CrowdactionParticipantCounts");
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionTag", b =>
                {
                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.Property<int>("CrowdactionId")
                        .HasColumnType("integer");

                    b.HasKey("TagId", "CrowdactionId");

                    b.HasIndex("CrowdactionId");

                    b.ToTable("CrowdactionTags");
                });

            modelBuilder.Entity("CollAction.Models.DonationEventLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("EventData")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("DonationEventLog");
                });

            modelBuilder.Entity("CollAction.Models.ImageFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Filepath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Height")
                        .HasColumnType("integer");

                    b.Property<int>("Width")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ImageFiles");
                });

            modelBuilder.Entity("CollAction.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("character varying(30)")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasAlternateKey("Name");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("CollAction.Models.UserEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("EventData")
                        .IsRequired()
                        .HasColumnType("json");

                    b.Property<DateTime>("EventLoggedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserEvents");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FriendlyName")
                        .HasColumnType("text");

                    b.Property<string>("Xml")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("CollAction.Models.Crowdaction", b =>
                {
                    b.HasOne("CollAction.Models.ImageFile", "BannerImage")
                        .WithMany()
                        .HasForeignKey("BannerImageFileId");

                    b.HasOne("CollAction.Models.ImageFile", "CardImage")
                        .WithMany()
                        .HasForeignKey("CardImageFileId");

                    b.HasOne("CollAction.Models.ImageFile", "DescriptiveImage")
                        .WithMany()
                        .HasForeignKey("DescriptiveImageFileId");

                    b.HasOne("CollAction.Models.ApplicationUser", "Owner")
                        .WithMany("Crowdactions")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionCategory", b =>
                {
                    b.HasOne("CollAction.Models.Crowdaction", "Crowdaction")
                        .WithMany("Categories")
                        .HasForeignKey("CrowdactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionComment", b =>
                {
                    b.HasOne("CollAction.Models.Crowdaction", "Crowdaction")
                        .WithMany("Comments")
                        .HasForeignKey("CrowdactionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("CollAction.Models.ApplicationUser", "User")
                        .WithMany("CrowdactionComments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionParticipant", b =>
                {
                    b.HasOne("CollAction.Models.Crowdaction", "Crowdaction")
                        .WithMany("Participants")
                        .HasForeignKey("CrowdactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CollAction.Models.ApplicationUser", "User")
                        .WithMany("Participates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionParticipantCount", b =>
                {
                    b.HasOne("CollAction.Models.Crowdaction", "Crowdaction")
                        .WithOne("ParticipantCounts")
                        .HasForeignKey("CollAction.Models.CrowdactionParticipantCount", "CrowdactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CollAction.Models.CrowdactionTag", b =>
                {
                    b.HasOne("CollAction.Models.Crowdaction", "Crowdaction")
                        .WithMany("Tags")
                        .HasForeignKey("CrowdactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CollAction.Models.Tag", "Tag")
                        .WithMany("CrowdactionTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CollAction.Models.DonationEventLog", b =>
                {
                    b.HasOne("CollAction.Models.ApplicationUser", "User")
                        .WithMany("DonationEvents")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CollAction.Models.UserEvent", b =>
                {
                    b.HasOne("CollAction.Models.ApplicationUser", "User")
                        .WithMany("UserEvents")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CollAction.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CollAction.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CollAction.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("CollAction.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
