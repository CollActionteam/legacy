using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class DescriptionVideoLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "DescriptionVideoLinkId",
                table: "Projects",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_VideoLinks_DescriptionVideoLinkId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DescriptionVideoLinkId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DescriptionVideoLinkId",
                table: "Projects");

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
    }
}
