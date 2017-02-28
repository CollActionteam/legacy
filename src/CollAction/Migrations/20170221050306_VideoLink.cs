using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class VideoLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoLinks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Date = table.Column<DateTime>(nullable: false),
                    Link = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLinks", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "VideoLinkId",
                table: "Projects",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_VideoLinkId",
                table: "Projects",
                column: "VideoLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_VideoLinks_VideoLinkId",
                table: "Projects",
                column: "VideoLinkId",
                principalTable: "VideoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_VideoLinks_VideoLinkId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_VideoLinkId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "VideoLinkId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "VideoLinks");
        }
    }
}
