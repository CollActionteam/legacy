using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class VideoCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_VideoLinks_DescriptionVideoLinkId",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_VideoLinks_DescriptionVideoLinkId",
                table: "Projects",
                column: "DescriptionVideoLinkId",
                principalTable: "VideoLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_VideoLinks_DescriptionVideoLinkId",
                table: "Projects");

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
