using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CollAction.Migrations
{
    public partial class SimplifyVideoLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionVideoLink",
                table: "Projects",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE public.""Projects"" AS ""P""
                  SET ""DescriptionVideoLink"" =
                  (
                      SELECT ""Link""
                      FROM public.""VideoLinks"" AS ""V""
                      WHERE ""P"".""DescriptionVideoLinkId"" = ""V"".""Id""
                  )");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_VideoLinks_DescriptionVideoLinkId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "VideoLinks");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DescriptionVideoLinkId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DescriptionVideoLinkId",
                table: "Projects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionVideoLink",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "DescriptionVideoLinkId",
                table: "Projects",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VideoLinks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Link = table.Column<string>(maxLength: 2083, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLinks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DescriptionVideoLinkId",
                table: "Projects",
                column: "DescriptionVideoLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_VideoLinks_DescriptionVideoLinkId",
                table: "Projects",
                column: "DescriptionVideoLinkId",
                principalTable: "VideoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
