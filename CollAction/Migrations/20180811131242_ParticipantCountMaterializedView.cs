using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class ParticipantCountMaterializedView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE MATERIALIZED VIEW ""ProjectParticipantCounts"" AS
                  SELECT proj.""Id"" AS ""ProjectId"",
                         proj.""AnonymousUserParticipants"" + COALESCE(SUM(usr.""RepresentsNumberParticipants""), 0) AS ""Count""
                  FROM public.""Projects"" AS proj
                  LEFT OUTER JOIN public.""ProjectParticipants"" AS part ON proj.""Id"" = part.""ProjectId""
                  LEFT OUTER JOIN public.""AspNetUsers"" AS usr ON part.""UserId"" = usr.""Id""
                  GROUP BY proj.""Id"";");

            migrationBuilder.CreateIndex("ProjectIdIndex", "ProjectParticipantCounts", "ProjectId", null, true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("ProjectIdIndex", "ProjectParticipantCounts");

            migrationBuilder.Sql(@"DROP MATERIALIZED VIEW ""ProjectParticipantCounts"";");
        }
    }
}
