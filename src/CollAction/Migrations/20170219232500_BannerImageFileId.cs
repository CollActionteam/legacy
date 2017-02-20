using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CollAction.Migrations
{
    public partial class BannerImageFileId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ImageFiles_ImageFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ImageFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ImageFileId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "BannerImageFileId",
                table: "Projects",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BannerImageFileId",
                table: "Projects",
                column: "BannerImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ImageFiles_BannerImageFileId",
                table: "Projects",
                column: "BannerImageFileId",
                principalTable: "ImageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ImageFiles_BannerImageFileId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_BannerImageFileId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "BannerImageFileId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "ImageFileId",
                table: "Projects",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ImageFileId",
                table: "Projects",
                column: "ImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ImageFiles_ImageFileId",
                table: "Projects",
                column: "ImageFileId",
                principalTable: "ImageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
