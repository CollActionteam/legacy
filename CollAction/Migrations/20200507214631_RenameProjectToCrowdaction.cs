using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class RenameProjectToCrowdaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW \"ProjectParticipantCounts\"");

            migrationBuilder.RenameColumn("NumberProjectEmailsSent", "Projects", "NumberCrowdactionEmailsSent");
            migrationBuilder.RenameColumn("SubscribedToProjectEmails", "ProjectParticipants", "SubscribedToCrowdactionEmails");
            migrationBuilder.RenameColumn("ProjectId", "ProjectParticipants", "CrowdactionId");
            migrationBuilder.RenameColumn("ProjectId", "ProjectTags", "CrowdactionId");
            migrationBuilder.RenameColumn("ProjectId", "ProjectCategories", "CrowdactionId");

            migrationBuilder.RenameIndex("IX_ProjectCategories_ProjectId", "IX_CrowdactionCategories_CrowdactionId", "ProjectCategories");
            migrationBuilder.RenameIndex("IX_ProjectParticipants_ProjectId", "IX_CrowdactionParticipants_CrowdactionId", "ProjectParticipants");
            migrationBuilder.RenameIndex("IX_ProjectTags_ProjectId", "IX_CrowdactionTags_CrowdactionId", "ProjectTags");
            migrationBuilder.RenameIndex("IX_Projects_BannerImageFileId", "IX_Crowdactions_BannerImageFileId", "Projects");
            migrationBuilder.RenameIndex("IX_Projects_CardImageFileId", "IX_Crowdactions_CardImageFileId", "Projects");
            migrationBuilder.RenameIndex("IX_Projects_DescriptiveImageFileId", "IX_Crowdactions_DescriptiveImageFileId", "Projects");
            migrationBuilder.RenameIndex("IX_Projects_Name", "IX_Crowdactions_Name", "Projects");
            migrationBuilder.RenameIndex("IX_Projects_OwnerId", "IX_Crowdactions_OwnerId", "Projects");

            migrationBuilder.Sql("ALTER TABLE \"ProjectCategories\" RENAME CONSTRAINT \"PK_ProjectCategories\" TO \"PK_CrowdactionCategories\"");
            migrationBuilder.Sql("ALTER TABLE \"ProjectCategories\" RENAME CONSTRAINT \"FK_ProjectCategories_Projects_ProjectId\" TO \"FK_CrowdactionCategories_Crowdactions_CrowdactionId\"");

            migrationBuilder.Sql("ALTER TABLE \"ProjectParticipants\" RENAME CONSTRAINT \"PK_ProjectParticipants\" TO \"PK_CrowdactionParticipants\"");
            migrationBuilder.Sql("ALTER TABLE \"ProjectParticipants\" RENAME CONSTRAINT \"FK_ProjectParticipants_Projects_ProjectId\" TO \"FK_CrowdactionParticipants_Crowdactions_CrowdactionId\"");
            migrationBuilder.Sql("ALTER TABLE \"ProjectParticipants\" RENAME CONSTRAINT \"FK_ProjectParticipants_AspNetUsers_UserId\" TO \"FK_CrowdactionParticipants_AspNetUsers_UserId\"");

            migrationBuilder.Sql("ALTER TABLE \"ProjectTags\" RENAME CONSTRAINT \"PK_ProjectTags\" TO \"PK_CrowdactionTags\"");
            migrationBuilder.Sql("ALTER TABLE \"ProjectTags\" RENAME CONSTRAINT \"FK_ProjectTags_Tags_TagId\" TO \"FK_CrowdactionTags_Tags_TagId\"");
            migrationBuilder.Sql("ALTER TABLE \"ProjectTags\" RENAME CONSTRAINT \"FK_ProjectTags_Projects_ProjectId\" TO \"FK_CrowdactionTags_Crowdactions_CrowdactionId\"");

            migrationBuilder.Sql("ALTER TABLE \"Projects\" RENAME CONSTRAINT \"PK_Projects\" TO \"PK_Crowdactions\"");
            migrationBuilder.Sql("ALTER TABLE \"Projects\" RENAME CONSTRAINT \"FK_Projects_AspNetUsers_OwnerId\" TO \"FK_Crowdactions_AspNetUsers_OwnerId\"");
            migrationBuilder.Sql("ALTER TABLE \"Projects\" RENAME CONSTRAINT \"FK_Projects_ImageFiles_BannerImageFileId\" TO \"FK_Crowdactions_ImageFiles_BannerImageFileId\"");
            migrationBuilder.Sql("ALTER TABLE \"Projects\" RENAME CONSTRAINT \"FK_Projects_ImageFiles_CardImageFileId\" TO \"FK_Crowdactions_ImageFiles_CardImageFileId\"");
            migrationBuilder.Sql("ALTER TABLE \"Projects\" RENAME CONSTRAINT \"FK_Projects_ImageFiles_DescriptiveImageFileId\" TO \"FK_Crowdactions_ImageFiles_DescriptiveImageFileId\"");

            migrationBuilder.RenameTable("Projects", newName: "Crowdactions");
            migrationBuilder.RenameTable("ProjectCategories", newName: "CrowdactionCategories");
            migrationBuilder.RenameTable("ProjectParticipants", newName: "CrowdactionParticipants");
            migrationBuilder.RenameTable("ProjectTags", newName: "CrowdactionTags");

            migrationBuilder.Sql(
                @"CREATE MATERIALIZED VIEW ""CrowdactionParticipantCounts"" AS
                              SELECT crowd.""Id"" AS ""CrowdactionId"",
                                     crowd.""AnonymousUserParticipants"" + COALESCE(SUM(usr.""RepresentsNumberParticipants""), 0) AS ""Count""
                              FROM public.""Crowdactions"" AS crowd
                              LEFT OUTER JOIN public.""CrowdactionParticipants"" AS part ON crowd.""Id"" = part.""CrowdactionId""
                              LEFT OUTER JOIN public.""AspNetUsers"" AS usr ON part.""UserId"" = usr.""Id""
                              GROUP BY crowd.""Id"";
                 CREATE UNIQUE INDEX ON ""CrowdactionParticipantCounts""(""CrowdactionId"")");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP MATERIALIZED VIEW \"CrowdactionParticipantCounts\"");

            migrationBuilder.RenameColumn("NumberCrowdactionEmailsSent", "Crowdactions", "NumberProjectEmailsSent");
            migrationBuilder.RenameColumn("SubscribedToCrowdactionEmails", "CrowdactionParticipants", "SubscribedToProjectEmails");
            migrationBuilder.RenameColumn("CrowdactionId", "CrowdactionParticipants", "ProjectId");
            migrationBuilder.RenameColumn("CrowdactionId", "CrowdactionTags", "ProjectId");
            migrationBuilder.RenameColumn("CrowdactionId", "CrowdactionCategories", "ProjectId");

            migrationBuilder.RenameIndex("IX_CrowdactionCategories_CrowdactionId", "IX_ProjectCategories_ProjectId", "CrowdactionCategories");
            migrationBuilder.RenameIndex("IX_CrowdactionParticipants_CrowdactionId", "IX_ProjectParticipants_ProjectId", "CrowdactionParticipants");
            migrationBuilder.RenameIndex("IX_CrowdactionTags_CrowdactionId", "IX_ProjectTags_ProjectId", "CrowdactionTags");
            migrationBuilder.RenameIndex("IX_Crowdactions_BannerImageFileId", "IX_Projects_BannerImageFileId", "Crowdactions");
            migrationBuilder.RenameIndex("IX_Crowdactions_CardImageFileId", "IX_Projects_CardImageFileId", "Crowdactions");
            migrationBuilder.RenameIndex("IX_Crowdactions_DescriptiveImageFileId", "IX_Projects_DescriptiveImageFileId", "Crowdactions");
            migrationBuilder.RenameIndex("IX_Crowdactions_Name", "IX_Projects_Name", "Crowdactions");
            migrationBuilder.RenameIndex("IX_Crowdactions_OwnerId", "IX_Projects_OwnerId", "Crowdactions");

            migrationBuilder.Sql("ALTER TABLE \"CrowdactionCategories\" RENAME CONSTRAINT \"PK_CrowdactionCategories\" TO \"PK_ProjectCategories\"");
            migrationBuilder.Sql("ALTER TABLE \"CrowdactionCategories\" RENAME CONSTRAINT \"FK_CrowdactionCategories_Crowdactions_CrowdactionId\" TO \"FK_ProjectCategories_Projects_ProjectId\"");

            migrationBuilder.Sql("ALTER TABLE \"CrowdactionParticipants\" RENAME CONSTRAINT \"PK_CrowdactionParticipants\" TO \"PK_ProjectParticipants\"");
            migrationBuilder.Sql("ALTER TABLE \"CrowdactionParticipants\" RENAME CONSTRAINT \"FK_CrowdactionParticipants_Crowdactions_CrowdactionId\" TO \"FK_ProjectParticipants_Projects_ProjectId\"");
            migrationBuilder.Sql("ALTER TABLE \"CrowdactionParticipants\" RENAME CONSTRAINT \"FK_CrowdactionParticipants_AspNetUsers_UserId\" TO \"FK_ProjectParticipants_AspNetUsers_UserId\"");

            migrationBuilder.Sql("ALTER TABLE \"CrowdactionTags\" RENAME CONSTRAINT \"PK_CrowdactionTags\" TO \"PK_ProjectTags\"");
            migrationBuilder.Sql("ALTER TABLE \"CrowdactionTags\" RENAME CONSTRAINT \"FK_CrowdactionTags_Tags_TagId\" TO \"FK_ProjectTags_Tags_TagId\"");
            migrationBuilder.Sql("ALTER TABLE \"CrowdactionTags\" RENAME CONSTRAINT \"FK_CrowdactionTags_Crowdactions_CrowdactionId\" TO \"FK_ProjectTags_Projects_ProjectId\"");

            migrationBuilder.Sql("ALTER TABLE \"Crowdactions\" RENAME CONSTRAINT \"PK_Crowdactions\" TO \"PK_Projects\"");
            migrationBuilder.Sql("ALTER TABLE \"Crowdactions\" RENAME CONSTRAINT \"FK_Crowdactions_AspNetUsers_OwnerId\" TO \"FK_Projects_AspNetUsers_OwnerId\"");
            migrationBuilder.Sql("ALTER TABLE \"Crowdactions\" RENAME CONSTRAINT \"FK_Crowdactions_ImageFiles_BannerImageFileId\" TO \"FK_Projects_ImageFiles_BannerImageFileId\"");
            migrationBuilder.Sql("ALTER TABLE \"Crowdactions\" RENAME CONSTRAINT \"FK_Crowdactions_ImageFiles_CardImageFileId\" TO \"FK_Projects_ImageFiles_CardImageFileId\"");
            migrationBuilder.Sql("ALTER TABLE \"Crowdactions\" RENAME CONSTRAINT \"FK_Crowdactions_ImageFiles_DescriptiveImageFileId\" TO \"FK_Projects_ImageFiles_DescriptiveImageFileId\"");

            migrationBuilder.RenameTable("Crowdactions", newName: "Projects");
            migrationBuilder.RenameTable("CrowdactionCategories", newName: "ProjectCategories");
            migrationBuilder.RenameTable("CrowdactionParticipants", newName: "ProjectParticipants");
            migrationBuilder.RenameTable("CrowdactionTags", newName: "ProjectTags");

            migrationBuilder.Sql(
                @"CREATE MATERIALIZED VIEW ""ProjectParticipantCounts"" AS
                              SELECT proj.""Id"" AS ""ProjectId"",
                                     proj.""AnonymousUserParticipants"" + COALESCE(SUM(usr.""RepresentsNumberParticipants""), 0) AS ""Count""
                              FROM public.""Projects"" AS proj
                              LEFT OUTER JOIN public.""ProjectParticipants"" AS part ON proj.""Id"" = part.""ProjectId""
                              LEFT OUTER JOIN public.""AspNetUsers"" AS usr ON part.""UserId"" = usr.""Id""
                              GROUP BY proj.""Id"";
                 CREATE UNIQUE INDEX ON ""ProjectParticipantCounts""(""ProjectId"")");
        }
    }
}
